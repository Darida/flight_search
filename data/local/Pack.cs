using System;
using System.Collections.ObjectModel;

namespace local {
        class Pack {
            public string UUID { get; set; }
            public string OutboundLegUUID { get; set; }
            public string ShortInboundLegUUID { get; set; }
            public string LongInboundLegUUID { get; set; }
            public double Price { get; set; }

            public string ShortDeeplink { get; set; }
            public string LongDeeplink { get; set; } 
            public string ShortAgent { get; set; }       
            public string LongAgent { get; set; }    
        }
}