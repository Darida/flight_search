using System;

namespace skyscanner {
        class Place {
            public int Id { get; set; }
            public int ParentId { get; set; }
            public String Code { get; set; }
            public String Type { get; set; }
            public String Name { get; set; }
        }
}