using System.Threading;
using System.Threading.Tasks;
using System;
using System.Linq;
using Flurl;
using Flurl.Http;
using System.Net.Http;
using Newtonsoft.Json;
using skyscanner;
using local;

namespace core
{
    class FindFlightsRunner
    {     
        public static readonly Storage STORAGE = Storage.STORAGE;
        private readonly Random RND = new Random();

        public async Task run(string originPlace, string destinationPlace, DateTime outboundDate, DateTime inboundDate, int adults)
        {
            string location = "";
            for(int delay = 10; location == ""; delay = (int) (delay * (RND.NextDouble() + 1.5))) {
                try {
                    location = (await "https://skyscanner-skyscanner-flight-search-v1.p.rapidapi.com/apiservices/pricing/v1.0"
                        .WithHeader("X-RapidAPI-Host", "skyscanner-skyscanner-flight-search-v1.p.rapidapi.com")
                        .WithHeader("X-RapidAPI-Key", "091026a234msh37b1d7bb1187eb4p12d4fcjsnec7945919c90")
                        .WithHeader("Content-Type", "application/x-www-form-urlencoded")   
                        .PostUrlEncodedAsync(new
                        {
                            cabinClass = "economy",
                            children = 0,
                            infants = 0,
                            country = "US",
                            currency = "USD",
                            locale = "en-US",
                            originPlace = originPlace + "-sky",
                            destinationPlace = destinationPlace + "-sky",
                            outboundDate = outboundDate.ToString("yyyy-MM-dd"),
                            inboundDate = inboundDate.ToString("yyyy-MM-dd"),
                            adults = adults,
                            groupPricing = true
                        })).Headers.Location.AbsolutePath;                    
                } catch (Exception ex) {
                    Console.WriteLine($"Request {originPlace}/{destinationPlace} {outboundDate.ToShortDateString()}/{inboundDate.ToShortDateString()} failed: {ex}");
                    await Task.Delay(TimeSpan.FromSeconds(delay));
                }
            }
            
            SkyResponse response = new SkyResponse();
            for(int delay = 10; response.Status != "UpdatesComplete"; delay = (int) (delay * (RND.NextDouble() + 1.5))) {
                Console.WriteLine($"Incomplete {location}. Waiting {delay}s before re-request.");
                await Task.Delay(TimeSpan.FromSeconds(delay));
                try {
                    response = await $"https://skyscanner-skyscanner-flight-search-v1.p.rapidapi.com/{location}"
                        .WithHeader("X-RapidAPI-Host", "skyscanner-skyscanner-flight-search-v1.p.rapidapi.com")
                        .WithHeader("X-RapidAPI-Key", "091026a234msh37b1d7bb1187eb4p12d4fcjsnec7945919c90")
                        .SetQueryParam("stops", 2)
                        .SetQueryParam("pageIndex", 0)
                        .SetQueryParam("pageSize", 999999)
                        .SetQueryParam("sortType", "price")
                        .GetJsonAsync<SkyResponse>();
                } catch (Exception ex) {
                    Console.WriteLine($"Request {location} failed: {ex}");
                }
            }

            Converter converter = new Converter();
            converter.remember(response.Carriers);
            converter.remember(response.Places);
            converter.remember(response.Agents);
            foreach (var segment in response.Segments)
            {
                STORAGE.save(converter.convert(segment));
            }
            foreach (var legs in response.Legs)
            {
                STORAGE.save(converter.convert(legs));
            }
            foreach (var itinerary in response.Itineraries)
            {
                STORAGE.save(converter.convert(itinerary));
            }
        }
    }
}
