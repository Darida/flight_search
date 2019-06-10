
using System.Collections.ObjectModel;
using System.Linq;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using local;

namespace core {
    class Storage {
        private static readonly string FOLDER = @"d:\tmp";        
        public static readonly Storage STORAGE = new Storage();

        public IDictionary<string, Segment> segments = new Dictionary<string, Segment>();
        public IDictionary<string, Leg> legs = new Dictionary<string, Leg>();
        public IDictionary<string, Itinerary> itineraries = new Dictionary<string, Itinerary>();

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
            itineraries[itinerary.UUID] = itinerary;
        }

        Storage() {
            loadFromDist();
        }

        private void loadFromDist() {
            try {
                segments = JsonConvert.DeserializeObject<ConcurrentDictionary<string, Segment>>(System.IO.File.ReadAllText($@"{FOLDER}\segments.txt"));
                legs = JsonConvert.DeserializeObject<ConcurrentDictionary<string, Leg>>(System.IO.File.ReadAllText($@"{FOLDER}\legs.txt"));
                itineraries = JsonConvert.DeserializeObject<ConcurrentDictionary<string, Itinerary>>(System.IO.File.ReadAllText($@"{FOLDER}\itineraries.txt"));
            } catch {
                // Ignore
            }
        }

        public void saveToDisk() {
            System.IO.File.WriteAllText($@"{FOLDER}\segments.txt", JsonConvert.SerializeObject(segments));
            System.IO.File.WriteAllText($@"{FOLDER}\legs.txt", JsonConvert.SerializeObject(legs));
            System.IO.File.WriteAllText($@"{FOLDER}\itineraries.txt", JsonConvert.SerializeObject(itineraries));
        }

        public DateTime GetLastUpdated(DateTime dateGoThere, DateTime dateReturn, string airportFrom, string airportTo)
        {
            return itineraries.Values
                .Where(it => legs[it.OutboundLegUUID].Origin == airportFrom)
                .Where(it => legs[it.OutboundLegUUID].Destination == airportTo)
                .Where(it => legs[it.OutboundLegUUID].Departure.Date == dateGoThere)
                .Where(it => legs[it.InboundLegUUID].Departure.Date == dateReturn)
                .Select(it => it.LastUpdated)
                .Concat(new DateTime[] { DateTime.MinValue })
                .Max();
        }
    }
}