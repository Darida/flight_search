using System;
using System.Collections.ObjectModel;

namespace local {
        class Itinerary {
            public String UUID { get; set; }
            public String OutboundLegUUID { get; set; }
            public String InboundLegUUID { get; set; }
            public double Price { get; set; }
            public String Deeplink { get; set; }
        }
}