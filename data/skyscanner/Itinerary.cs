using System;
using System.Collections.Generic;

namespace skyscanner {
        class Itinerary {
            public string OutboundLegId { get; set; }
            public string InboundLegId { get; set; }
            public ICollection<PricingOption> PricingOptions { get; set; }
        }
}