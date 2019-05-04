using System;
using System.Collections.ObjectModel;

namespace skyscanner {
        class Leg {
            public string Id { get; set; }
            public Collection<int> SegmentIds { get; set; }
            public int OriginStation { get; set; }
            public int DestinationStation { get; set; }
            public DateTime Departure { get; set; }
            public DateTime Arrival { get; set; }
            public int Duration { get; set; }
        }
}