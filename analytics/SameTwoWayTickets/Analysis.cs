
using System.Collections.ObjectModel;
using System.Linq;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using local;
using core;

namespace analytics.SameTwoWayTickets {
    class Analysis {   
        public static readonly Storage STORAGE = Storage.STORAGE;
        
        public void report() {    
            Console.WriteLine($"Looking for best options...");  

            List<Itinerary> all = STORAGE.itineraries.Values
                .Where(it => STORAGE.legs[it.OutboundLegUUID].SegmentUUIDs.Count == 1)
                .Where(it => STORAGE.legs[it.InboundLegUUID].SegmentUUIDs.Count == 1)
                .Where(it => getVacationDays(it) <= 3)
                .OrderBy(it => it.Price)            
                .ToList();
            
            for(int days = 1; all.Count > 0; days++) {
                Console.WriteLine($"For trips with duration at least {days - 1} days:");
                foreach(Itinerary optimal in all.Take(5)) {
                    Console.WriteLine($"Best (from {all.Count}): {toString(optimal)}");    
                }

                all = all
                    .Where(it => getDuration(it) > TimeSpan.FromDays(days))
                    .ToList();
            }
        }

        private string toString(Itinerary itinerary) {
            return $"Price: {itinerary.Price}$. Agent: {itinerary.Agent}. "
                + $"Duration: {getDuration(itinerary).Days} days {getDuration(itinerary).Hours} hours. "
                + $"Vacation: {getVacationDays(itinerary)} days. "
                + $"From: {STORAGE.legs[itinerary.OutboundLegUUID].Departure.ToShortDateString()}. To: {STORAGE.legs[itinerary.InboundLegUUID].Departure.ToShortDateString()}. " ;
        }

        private TimeSpan getDuration(Itinerary itinerary) {
            return STORAGE.legs[itinerary.InboundLegUUID].Departure - STORAGE.legs[itinerary.OutboundLegUUID].Arrival;
        }
        private int getVacationDays(Itinerary itinerary) {
            return Vacations.calculate(STORAGE.legs[itinerary.OutboundLegUUID].Departure, STORAGE.legs[itinerary.InboundLegUUID].Arrival);
        }
    }
}