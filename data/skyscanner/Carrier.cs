using System;

namespace skyscanner {
        class Carrier {
            public int Id { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }
            public Uri ImageUrl { get; set; }
            public string DisplayCode { get; set; }
        }
}