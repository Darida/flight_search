using System;
using System.Collections.ObjectModel;

namespace analytics.SameOneWayTicketForTwoAdults {
    class Pack {
        public string UUID { get; set; }
        public string OutboundLegUUID { get; set; }
        public string FirstInboundLegUUID { get; set; }
        public string SecondInboundLegUUID { get; set; }
        public double Price { get; set; }

        public string FirstDeeplink { get; set; }
        public string SecondDeeplink { get; set; } 
        public string FirstAgent { get; set; }       
        public string SecondAgent { get; set; }    
    }
}