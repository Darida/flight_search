using System;
using System.Collections.Generic;

namespace skyscanner {
    class SkyResponse {
        public String SessionKey { get; set; }
        public String Status { get; set; }
        public ICollection<Itinerary> Itineraries { get; set; }
        public ICollection<Leg> Legs { get; set; }
        public ICollection<Segment> Segments { get; set; }
        public ICollection<Carrier> Carriers { get; set; }
        public ICollection<Place> Places { get; set; }
        public ICollection<Agent> Agents { get; set; }
    }
}