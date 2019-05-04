using System.Collections.Generic;

namespace skyscanner {
        class PricingOption {
            public double Price { get; set; }
            public int QuoteAgeInMinutes { get; set; }
            public string DeeplinkUrl { get; set; }
            public ICollection<int> Agents { get; set; }
        }
}