
using System.Collections.ObjectModel;
using System.Linq;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using local;

namespace core {
    class Storage {
        private static readonly string FOLDER = @"d:\flight";        
        public static readonly Storage STORAGE = new Storage();

        public IDictionary<string, Segment> segments = new Dictionary<string, Segment>();
        public IDictionary<string, Leg> legs = new Dictionary<string, Leg>();
        public IDictionary<string, Itinerary> longItineraries = new Dictionary<string, Itinerary>();
        public IDictionary<string, Itinerary> shortItineraries = new Dictionary<string, Itinerary>();

        public void save(Segment segment) {
            segments[segment.UUID] = segment;
        }
        public void save(Leg leg) {
            if(!legs.ContainsKey(leg.UUID)) {
                legs[leg.UUID] = leg;
            }
            // don't update if exists to keep isSelfTrasfer flag
        }
        public void save(Itinerary itinerary) {
            itinerary.LastUpdated = DateTime.Now;
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
            } catch {
                // Ignore
            }
        }

        public void saveToDisk() {
            System.IO.File.WriteAllText($@"{FOLDER}\segments.txt", JsonConvert.SerializeObject(segments));
            System.IO.File.WriteAllText($@"{FOLDER}\legs.txt", JsonConvert.SerializeObject(legs));
            System.IO.File.WriteAllText($@"{FOLDER}\long_itineraries.txt", JsonConvert.SerializeObject(longItineraries));
            System.IO.File.WriteAllText($@"{FOLDER}\short_itineraries.txt", JsonConvert.SerializeObject(shortItineraries));
        }
    }
}