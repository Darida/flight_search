using System;
using System.Collections.ObjectModel;

namespace local {
        class Pack {
            public String UUID { get; set; }
            public String OutboundLegUUID { get; set; }
            public String ShortInboundLegUUID { get; set; }
            public String LongInboundLegUUID { get; set; }
            public double Price { get; set; }
            public String ShortDeeplink { get; set; }
            public String LongDeeplink { get; set; }            
        }
}