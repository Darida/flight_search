
using System.Collections.ObjectModel;
using System.Linq;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using local;
using core;

namespace analytics.SameOneWayTicketForTwoAdults {
    class Analysis {      
        public static readonly Storage STORAGE = Storage.STORAGE;
        public List<Pack> packs = new List<Pack>();

        public void report() {    
            Console.WriteLine($"Looking for best options...");  
                 
            makeMatch();
            List<Pack> all = packs;

            /* foreach(Pack option in all) {
                Console.WriteLine(toString(option));
                Console.WriteLine(option.OutboundLegUUID);
                Console.WriteLine(option.ShortInboundLegUUID);
                Console.WriteLine(option.LongInboundLegUUID);
            }*/
            while(all.Count > 0) {
                Pack optimal = all.FirstOrDefault();
                Console.WriteLine($"Optimal (from {all.Count}): {toString(optimal)}");
                Console.WriteLine(optimal.OutboundLegUUID);
                Console.WriteLine(optimal.FirstInboundLegUUID);
                Console.WriteLine(optimal.SecondInboundLegUUID);
                            
                double current = //legs[optimal.OutboundLegUUID].DurationSpan.TotalHours;
                        Math.Max(Math.Max(
                            STORAGE.legs[optimal.OutboundLegUUID].DurationSpan.TotalHours,
                            STORAGE.legs[optimal.FirstInboundLegUUID].DurationSpan.TotalHours),
                            STORAGE.legs[optimal.SecondInboundLegUUID].DurationSpan.TotalHours);
                int maxDuration = (int)Math.Floor(current);  

                all = all
                    .Where(it => STORAGE.legs[it.OutboundLegUUID].DurationSpan.TotalHours < maxDuration)
                    .Where(it => STORAGE.legs[it.FirstInboundLegUUID].DurationSpan.TotalHours < maxDuration)
                    .Where(it => STORAGE.legs[it.SecondInboundLegUUID].DurationSpan.TotalHours < maxDuration)
                    .ToList();  
            }
        }

        public void makeMatch() {
            packs.Clear();

            ILookup<string, Itinerary> longGroup = STORAGE.itineraries.Values
                .Where(it => it.Price < 3000)
                .Where(it => STORAGE.legs[it.OutboundLegUUID].DurationSpan.TotalHours < 24)
                .Where(it => STORAGE.legs[it.InboundLegUUID].DurationSpan.TotalHours < 24)
                .ToLookup(it => it.OutboundLegUUID);
            ILookup<string, Itinerary> shortGroup = STORAGE.itineraries.Values
                .Where(it => it.Price < 3000)
                .Where(it => STORAGE.legs[it.OutboundLegUUID].DurationSpan.TotalHours < 24)
                .Where(it => STORAGE.legs[it.InboundLegUUID].DurationSpan.TotalHours < 24)
                .ToLookup(it => it.OutboundLegUUID);

            foreach(IGrouping<string, Itinerary> group in shortGroup) {
                if(longGroup[group.Key] == null) continue;

                foreach (Itinerary longIt in longGroup[group.Key]) {
                    foreach (Itinerary shortIt in group) {
                        Pack pack = new Pack() {
                            UUID = $"{shortIt.OutboundLegUUID} / {shortIt.InboundLegUUID} / {longIt.InboundLegUUID}",
                            OutboundLegUUID = shortIt.OutboundLegUUID,
                            FirstInboundLegUUID  = shortIt.InboundLegUUID,
                            SecondInboundLegUUID  = longIt.InboundLegUUID,
                            Price  = shortIt.Price + longIt.Price,
                            FirstDeeplink = shortIt.Deeplink,
                            SecondDeeplink = longIt.Deeplink,
                            FirstAgent = shortIt.Agent,
                            SecondAgent = longIt.Agent
                        };
                        packs.Add(pack);
                    }                    
                }
            }

            packs = packs
                .Where(it => it.Price < 4000)
                .Where(it => getLongStayDuration(it) > TimeSpan.FromDays(21))
                .Where(it => getShortStayDuration(it) > TimeSpan.FromDays(6))
                //.Where(it => getVacationDays(it) < 8)
                .Where(it => STORAGE.legs[it.SecondInboundLegUUID].Arrival < new DateTime(2019, 6, 22))
                .OrderBy(it => it.Price).ToList();
        }

        private TimeSpan getLongStayDuration(Pack pack) {
            return STORAGE.legs[pack.SecondInboundLegUUID].Departure - STORAGE.legs[pack.OutboundLegUUID].Arrival;
        }

        private TimeSpan getShortStayDuration(Pack pack) {
            return STORAGE.legs[pack.FirstInboundLegUUID].Departure - STORAGE.legs[pack.OutboundLegUUID].Arrival;
        }

        private int getVacationDays(Pack pack) {
            int vacations = 0;
            for(DateTime day = STORAGE.legs[pack.OutboundLegUUID].Departure; day <= STORAGE.legs[pack.FirstInboundLegUUID].Arrival; day = day.Add(TimeSpan.FromDays(1))) {
                if(day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday)
                    continue;
                vacations++;
            }
            return vacations;
        }

        private string toString(Pack pack) {
            return $"{pack.Price}$ from {pack.SecondAgent}/{pack.FirstAgent} "
                + $"{STORAGE.legs[pack.OutboundLegUUID].Departure.ToShortDateString()} / {STORAGE.legs[pack.FirstInboundLegUUID].Departure.ToShortDateString()} / {STORAGE.legs[pack.SecondInboundLegUUID].Departure.ToShortDateString()}. " 
                + $"{STORAGE.legs[pack.OutboundLegUUID].SegmentUUIDs.Count - 1} & {STORAGE.legs[pack.FirstInboundLegUUID].SegmentUUIDs.Count - 1} & {STORAGE.legs[pack.SecondInboundLegUUID].SegmentUUIDs.Count - 1} stops. "
                + $"{STORAGE.legs[pack.OutboundLegUUID].DurationSpan} & {STORAGE.legs[pack.FirstInboundLegUUID].DurationSpan} & {STORAGE.legs[pack.SecondInboundLegUUID].DurationSpan} duration. ";
        }
    }
}