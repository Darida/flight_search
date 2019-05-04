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
            //analysis.report();
            await new CheckIsSelfTransferRunner().isSelfTransferAsync(Storage.STORAGE.shortItineraries.Values.First());
        }

        static void Main(string[] args)
        { 
            FindFlightsRunner runner = new FindFlightsRunner();

             for(int dayInMay = 17; dayInMay <= 17; dayInMay++) {       
                Console.WriteLine($"Processing {dayInMay} May"); 
                List<Task> tasks = new List<Task>(); 
                foreach(String airport in new string[] {"SFO-sky", "OAK-sky", "SJC-sky"})  {
                    for(int daysThere = 5; daysThere < 12; ++daysThere) {    
                        Task task = runner.run(airport, "MSQ-sky", new DateTime(2019, 5, dayInMay), new DateTime(2019, 5, dayInMay).AddDays(daysThere));  
                        tasks.Add(task);   
                    }                
                    for(int dayInJune = 15; dayInJune <= 30; dayInJune++) {       
                        Task task = runner.run(airport, "MSQ-sky", new DateTime(2019, 5, dayInMay), new DateTime(2019, 6, dayInJune));   
                        tasks.Add(task);
                    }   
                }
                Task.WaitAll(tasks.ToArray());   
                Storage.STORAGE.saveToDisk();              
            }
        }
    }
}
