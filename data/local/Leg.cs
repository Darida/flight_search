using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace local {
        class Leg {
            public String UUID { get; set; }
            public ICollection<string> SegmentUUIDs { get; set; }
            public DateTime Departure { get; set; }
            public DateTime Arrival { get; set; }
            public int Duration { get; set; }

            [IgnoreDataMember]            
            public TimeSpan DurationSpan { get { return TimeSpan.FromMinutes(Duration); } }
        }
}