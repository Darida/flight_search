
using System.Collections.ObjectModel;
using System.Linq;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace local {
    class Storage {
        private static readonly string FOLDER = @"d:\flight";        
        public static readonly Storage STORAGE = new Storage();

        IDictionary<string, Segment> segments = new Dictionary<string, Segment>();
        IDictionary<string, Leg> legs = new Dictionary<string, Leg>();
        IDictionary<string, Itinerary> longItineraries = new Dictionary<string, Itinerary>();
        IDictionary<string, Itinerary> shortItineraries = new Dictionary<string, Itinerary>();
        IDictionary<string, Pack> packs = new Dictionary<string, Pack>();

        public void save(Segment segment) {
            segments[segment.UUID] = segment;
        }
        public void save(Leg leg) {
            legs[leg.UUID] = leg;
        }
        public void save(Itinerary itinerary) {
            if(legs[itinerary.InboundLegUUID].Departure - legs[itinerary.OutboundLegUUID].Departure <= TimeSpan.FromDays(14)) {
                shortItineraries[itinerary.UUID] = itinerary;
            } else {
                longItineraries[itinerary.UUID] = itinerary;
            }
        }

        Storage() {
            loadFromDist();
        }

        public void loadFromDist() {
            try {
                segments = JsonConvert.DeserializeObject<ConcurrentDictionary<string, Segment>>(System.IO.File.ReadAllText($@"{FOLDER}\segments.txt"));
                legs = JsonConvert.DeserializeObject<ConcurrentDictionary<string, Leg>>(System.IO.File.ReadAllText($@"{FOLDER}\legs.txt"));
                longItineraries = JsonConvert.DeserializeObject<ConcurrentDictionary<string, Itinerary>>(System.IO.File.ReadAllText($@"{FOLDER}\long_itineraries.txt"));
                shortItineraries = JsonConvert.DeserializeObject<ConcurrentDictionary<string, Itinerary>>(System.IO.File.ReadAllText($@"{FOLDER}\short_itineraries.txt"));
                makeMatch();
            } catch {
                // Ignore
            }
        }

        public void saveToDisk() {
            System.IO.File.WriteAllText($@"{FOLDER}\segments.txt", JsonConvert.SerializeObject(segments));
            System.IO.File.WriteAllText($@"{FOLDER}\legs.txt", JsonConvert.SerializeObject(legs));
            System.IO.File.WriteAllText($@"{FOLDER}\long_itineraries.txt", JsonConvert.SerializeObject(longItineraries));
            System.IO.File.WriteAllText($@"{FOLDER}\short_itineraries.txt", JsonConvert.SerializeObject(shortItineraries));

            makeMatch();
            System.IO.File.WriteAllText($@"{FOLDER}\packs.txt", 
                JsonConvert.SerializeObject(packs.Values
                    .Where(it => legs[it.OutboundLegUUID].DurationSpan.TotalHours < 24)
                    .Where(it => legs[it.ShortInboundLegUUID].DurationSpan.TotalHours < 24)
                    .Where(it => legs[it.LongInboundLegUUID].DurationSpan.TotalHours < 24)
                    .OrderBy(it => it.Price).Take(1000)));

            report();
        }

        public void report() {               
            List<Pack> all = packs.Values.OrderBy(it => it.Price).ToList();
            while(all.Count > 0) {
                Pack optimal = all.FirstOrDefault();
                Console.WriteLine($"Optimal (from {all.Count}): {toString(optimal)}");
                Console.WriteLine(optimal.OutboundLegUUID);
                Console.WriteLine(optimal.ShortInboundLegUUID);
                Console.WriteLine(optimal.LongInboundLegUUID);
                            
                double current = //legs[optimal.OutboundLegUUID].DurationSpan.TotalHours;
                        Math.Max(Math.Max(
                            legs[optimal.OutboundLegUUID].DurationSpan.TotalHours,
                            legs[optimal.ShortInboundLegUUID].DurationSpan.TotalHours),
                            legs[optimal.LongInboundLegUUID].DurationSpan.TotalHours);
                int maxDuration = (int)Math.Floor(current);  

                all = packs.Values
                    .Where(it => legs[it.OutboundLegUUID].DurationSpan.TotalHours < maxDuration)
                    .Where(it => legs[it.ShortInboundLegUUID].DurationSpan.TotalHours < maxDuration)
                    .Where(it => legs[it.LongInboundLegUUID].DurationSpan.TotalHours < maxDuration)
                    .Where(it => getLongStayDuration(it) > TimeSpan.FromDays(20))
                    .Where(it => getShortStayDuration(it) > TimeSpan.FromDays(6))
                    .Where(it => getVacationDays(it) < 8)
                    .Where(it => legs[it.LongInboundLegUUID].Arrival < new DateTime(2019, 6, 22))
                    .OrderBy(it => it.Price).ToList();  
            }
        }

        private void makeMatch() {
            packs.Clear();

            ILookup<string, Itinerary> longGroup = longItineraries.Values.ToLookup(it => it.OutboundLegUUID);
            ILookup<string, Itinerary> shortGroup = shortItineraries.Values.ToLookup(it => it.OutboundLegUUID);

            foreach(IGrouping<string, Itinerary> group in shortGroup) {
                if(longGroup[group.Key] == null) continue;

                foreach (Itinerary longIt in longGroup[group.Key]) {
                    foreach (Itinerary shortIt in group) {
                        Pack pack = new Pack() {
                            UUID = $"{shortIt.OutboundLegUUID} / {shortIt.InboundLegUUID} / {longIt.InboundLegUUID}",
                            OutboundLegUUID = shortIt.OutboundLegUUID,
                            ShortInboundLegUUID  = shortIt.InboundLegUUID,
                            LongInboundLegUUID  = longIt.InboundLegUUID,
                            Price  = shortIt.Price + longIt.Price,
                            ShortDeeplink = shortIt.Deeplink,
                            LongDeeplink = longIt.Deeplink
                        };
                        packs[pack.UUID] = pack;
                    }                    
                }
            }
        }

        private TimeSpan getLongStayDuration(Pack pack) {
            return legs[pack.LongInboundLegUUID].Departure - legs[pack.OutboundLegUUID].Arrival;
        }

        private TimeSpan getShortStayDuration(Pack pack) {
            return legs[pack.ShortInboundLegUUID].Departure - legs[pack.OutboundLegUUID].Arrival;
        }

        private int getVacationDays(Pack pack) {
            int vacations = 0;
            for(DateTime day = legs[pack.OutboundLegUUID].Departure; day < legs[pack.ShortInboundLegUUID].Arrival; day = day.Add(TimeSpan.FromDays(1))) {
                if(day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday)
                    continue;
                vacations++;
            }
            return vacations;
        }

        private string toString(Pack pack) {
            return $"{pack.Price}$ at {legs[pack.OutboundLegUUID].Departure.ToShortDateString()} / {legs[pack.ShortInboundLegUUID].Departure.ToShortDateString()} / {legs[pack.LongInboundLegUUID].Departure.ToShortDateString()}. " 
                + $"{legs[pack.OutboundLegUUID].SegmentUUIDs.Count - 1} & {legs[pack.ShortInboundLegUUID].SegmentUUIDs.Count - 1} & {legs[pack.LongInboundLegUUID].SegmentUUIDs.Count - 1} stops. "
                + $"{legs[pack.OutboundLegUUID].DurationSpan} & {legs[pack.ShortInboundLegUUID].DurationSpan} & {legs[pack.LongInboundLegUUID].DurationSpan} duration. ";
        }
    }
}