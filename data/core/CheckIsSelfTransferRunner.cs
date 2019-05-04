using System;
using System.Collections.ObjectModel;
using local;

namespace core {
        class CheckIsSelfTransferRunner {   
            public string getUrl(Itinerary it) {
                return $@"https://www.skyscanner.com/transport/flights/" 
                    + $@"{Storage.STORAGE.legs[it.OutboundLegUUID].Origin}/{Storage.STORAGE.legs[it.OutboundLegUUID].Origin}"
                    + $@"/{Storage.STORAGE.legs[it.OutboundLegUUID].Departure.ToString("yyMMdd")}"
                    + $@"/{Storage.STORAGE.legs[it.InboundLegUUID].Departure.ToString("yyMMdd")}"
                    + $@"/?adults=1&children=0&adultsv2=1&childrenv2=&infants=0&cabinclass=economy&rtn=1&preferdirects=false&outboundaltsenabled=true&inboundaltsenabled=false&ref=home#details"
                    + $@"/{it.OutboundLegUUID}|{it.InboundLegUUID}";
            }      
        }
}