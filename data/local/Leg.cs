using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace local {
        class Leg {
            public string UUID { get; set; }
            public String Origin { get; set; }
            public String Destination { get; set; }
            public DateTime Departure { get; set; }
            public DateTime Arrival { get; set; }
            public int Duration { get; set; }
            public Boolean isSelfTrasfer {get; set;}

            public ICollection<string> SegmentUUIDs { get; set; }

            [IgnoreDataMember]            
            public TimeSpan DurationSpan { get { return TimeSpan.FromMinutes(Duration); } }
        }
}