using System.Threading.Tasks;
using System;
using System.Linq;
using Flurl;
using Flurl.Http;
using System.Net.Http;
using Newtonsoft.Json;
using skyscanner;
using local;
using System.Collections.Generic;

namespace core
{
    class Program
    {       

        static async Task Main(string[] args) {
            analytics.SameTwoWayTickets.Analysis analysis = new analytics.SameTwoWayTickets.Analysis();
            analysis.report();
        }

        private static readonly TimeSpan MIN_TIME_TO_REREQUEST = TimeSpan.FromHours(10);
        private static readonly TimeSpan SAVE_INTERVAL = TimeSpan.FromSeconds(30);

        private static readonly string[] AIRPORTS_FROM = new string[] {"SFO", "OAK", "SJC"};
        private static readonly string[] AIRPORTS_TO = new string[] {"LIH"};
        private static readonly DateTime START_GO_THERE = new DateTime(2019, 6, 20);
        private static readonly DateTime END_GO_THERE = new DateTime(2019, 7, 7);   
        private static readonly DateTime START_GO_BACK = new DateTime(2019, 6, 20);
        private static readonly DateTime END_GO_BACK = new DateTime(2019, 7, 7);

        public class SearchParam {
            public DateTime DateGoThere {get; set; }
            public DateTime DateReturn {get; set; }
            public String AirportFrom {get; set; }
            public String AirportTo {get; set; }
        }

        static async Task Main1(string[] args)
        {  
            List<SearchParam> searchSearchParams = new List<SearchParam>(); 
            for(DateTime goThereDate = START_GO_THERE; goThereDate <= END_GO_THERE; goThereDate = goThereDate.AddDays(1)) {
                for(DateTime returnDate = START_GO_BACK; returnDate <= END_GO_BACK; returnDate = returnDate.AddDays(1)) {
                    if(goThereDate >= returnDate) continue;

                    foreach(String fromAirport in AIRPORTS_FROM)  {
                        foreach(String toAirport in AIRPORTS_TO)  {
                            SearchParam param = new SearchParam() {
                                DateGoThere = goThereDate,
                                DateReturn = returnDate,
                                AirportFrom = fromAirport,
                                AirportTo = toAirport
                            };

                            DateTime lastUpdated = Storage.STORAGE.GetLastUpdated(param.DateGoThere, param.DateReturn, param.AirportFrom, param.AirportTo);
                            if((DateTime.Now - lastUpdated) < MIN_TIME_TO_REREQUEST) 
                                continue;

                            searchSearchParams.Add(param);
                        }
                    }
                }
            }

            DateTime lastSaved = DateTime.Now;
            FindFlightsRunner runner = new FindFlightsRunner();
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < searchSearchParams.Count; i++) {
                SearchParam param = searchSearchParams[i];

                Task task = runner.run(param.AirportFrom, param.AirportTo, param.DateGoThere, param.DateReturn, 2);  
                tasks.Add(task);
                
                if((DateTime.Now - lastSaved) > SAVE_INTERVAL) {
                    Storage.STORAGE.saveToDisk(); 
                    lastSaved = DateTime.Now;
                    Console.WriteLine($"Saved.");
                }

                int tasksInProcess = tasks.Count(t => !t.IsCompleted);
                Console.WriteLine($"{tasksInProcess} tasks in process ({searchSearchParams.Count - 1 - i} to go).");
                await Task.Delay(TimeSpan.FromSeconds(tasksInProcess/50));
            }            
            Task.WaitAll(tasks.ToArray());
            Storage.STORAGE.saveToDisk(); 
            Console.WriteLine($"Saved.");
        }
    }
}
