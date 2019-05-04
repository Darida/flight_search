using System;
using System.Collections.ObjectModel;

namespace skyscanner {
    class SkyResponse {
        public String SessionKey { get; set; }
        public String Status { get; set; }
        public Collection<Itinerary> Itineraries { get; set; }
        public Collection<Leg> Legs { get; set; }
        public Collection<Segment> Segments { get; set; }
        public Collection<Carrier> Carriers { get; set; }
        public Collection<Place> Places { get; set; }
    }
}