using System;

namespace skyscanner {
        class Segment {
            public int Id { get; set; }
            public int OriginStation { get; set; }
            public int DestinationStation { get; set; }
            public DateTime DepartureDateTime { get; set; }
            public DateTime ArrivalDateTime { get; set; }
            public int Carrier { get; set; }
            public int OperatingCarrier { get; set; }
            public int Duration { get; set; }
            public int FlightNumber { get; set; }
        }
}