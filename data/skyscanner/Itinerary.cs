using System;
using System.Collections.ObjectModel;

namespace skyscanner {
        class Itinerary {
            public String OutboundLegId { get; set; }
            public String InboundLegId { get; set; }
            public Collection<PricingOption> PricingOptions { get; set; }
        }
}