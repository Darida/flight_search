using System;

namespace local {
        class Segment {
            public String UUID { get; set; }

            public String Carrier { get; set; }    
            public String OperatingCarrier { get; set; }
            public int FlightNumber { get; set; }        

            public String Origin { get; set; }
            public String Destination { get; set; }
            public DateTime Departure { get; set; }
            public DateTime Arrival { get; set; }
            public int Duration { get; set; }
        }
}