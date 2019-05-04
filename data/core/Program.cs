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
        static async Task Main1(string[] args) {
            Storage.STORAGE.loadFromDist();
            Analysis analysis = new Analysis();
            analysis.makeMatch();
            analysis.report();
            //await new CheckIsSelfTransferRunner().isSelfTransferAsync(Storage.STORAGE.shortItineraries.Values.OrderBy(it => it.Price).First());
        }

        private static readonly TimeSpan MIN_TIME_TO_REREQUEST = TimeSpan.FromHours(10);
        static async Task Main(string[] args)
        { 
            FindFlightsRunner runner = new FindFlightsRunner();

            List<Task> tasks = new List<Task>(); 
             for(int dayInMay = 17; dayInMay <= 31; dayInMay++) {       
                Console.WriteLine($"Processing {dayInMay} May"); 
                foreach(String airport in new string[] {"SFO-sky", "OAK-sky", "SJC-sky"})  {
                    for(int daysThere = 5; daysThere < 16; ++daysThere) {   
                        DateTime goThereDate = new DateTime(2019, 5, dayInMay);   
                        DateTime returnDate = new DateTime(2019, 5, dayInMay).AddDays(daysThere);

                        DateTime lastUpdated = Storage.STORAGE.shortItineraries.Values
                            .Where(it => airport.Contains(Storage.STORAGE.legs[it.OutboundLegUUID].Origin))
                            .Where(it => Storage.STORAGE.legs[it.OutboundLegUUID].Departure >= goThereDate) 
                            .Where(it => (Storage.STORAGE.legs[it.OutboundLegUUID].Departure - goThereDate).TotalHours < 24) 
                            .Where(it => Storage.STORAGE.legs[it.InboundLegUUID].Departure >= returnDate) 
                            .Where(it => (Storage.STORAGE.legs[it.InboundLegUUID].Departure - returnDate).TotalHours < 24) 
                            .Select(it => it.LastUpdated)
                            .Concat(new DateTime[]{ DateTime.MinValue })
                            .Max();
                        if((DateTime.Now - lastUpdated) < MIN_TIME_TO_REREQUEST) 
                            continue;
                            
                        Task task = runner.run(airport, "MSQ-sky", goThereDate, returnDate);  
                        tasks.Add(task);   
                    }                
                    for(int dayInJune = 15; dayInJune <= 30; dayInJune++) {  
                        DateTime goThereDate = new DateTime(2019, 5, dayInMay);   
                        DateTime returnDate = new DateTime(2019, 6, dayInJune);

                        DateTime lastUpdated = Storage.STORAGE.longItineraries.Values
                            .Where(it => airport.Contains(Storage.STORAGE.legs[it.OutboundLegUUID].Origin))
                            .Where(it => (Storage.STORAGE.legs[it.OutboundLegUUID].Departure - goThereDate).TotalHours < 24) 
                            .Where(it => (Storage.STORAGE.legs[it.InboundLegUUID].Departure - returnDate).TotalHours < 24) 
                            .Select(it => it.LastUpdated)
                            .Concat(new DateTime[]{ DateTime.MinValue })
                            .Max();
                        if((DateTime.Now - lastUpdated) < MIN_TIME_TO_REREQUEST) 
                            continue;

                        Task task = runner.run(airport, "MSQ-sky", goThereDate, returnDate);   
                        tasks.Add(task);
                    } 
                    Storage.STORAGE.saveToDisk();

                    int delay = tasks.Count(t => !t.IsCompleted);
                    Console.WriteLine($"{delay} tasks in process.");
                    await Task.Delay(TimeSpan.FromSeconds(delay));  
                }            
            }
            Task.WaitAll(tasks.ToArray());
            Storage.STORAGE.saveToDisk(); 
        }
    }
}
