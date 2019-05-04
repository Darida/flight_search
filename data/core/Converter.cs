
using System.Linq;
using System.Numerics;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace core {
    class Converter {
        IDictionary<int, skyscanner.Carrier> carriers = new Dictionary<int, skyscanner.Carrier>();
        IDictionary<int, skyscanner.Place> places = new Dictionary<int, skyscanner.Place>();
        IDictionary<int, skyscanner.Agent> agents = new Dictionary<int, skyscanner.Agent>();
        IDictionary<int, string> segments = new Dictionary<int, string>();

        public void remember(ICollection<skyscanner.Carrier> carriers) {
            foreach(skyscanner.Carrier carrier in carriers) {
                this.carriers[carrier.Id] = carrier;
            }
        }
        public void remember(ICollection<skyscanner.Place> places) {
            foreach(skyscanner.Place place in places) {
                this.places[place.Id] = place;
            }
        }
        public void remember(ICollection<skyscanner.Agent> agents) {
            foreach(skyscanner.Agent agent in agents) {
                this.agents[agent.Id] = agent;
            }
        }

        public local.Segment convert(skyscanner.Segment other) {
            local.Segment that = new local.Segment() {
                Carrier = carriers[other.Carrier].Code,
                OperatingCarrier = carriers[other.OperatingCarrier].Code,
                FlightNumber = other.FlightNumber,

                Origin = places[other.OriginStation].Code,
                Destination = places[other.DestinationStation].Code,
                Departure = other.DepartureDateTime,
                Arrival = other.ArrivalDateTime,
                Duration = other.Duration
            };
            string uuid = $"{that.Carrier}{that.FlightNumber} at {that.Departure}";
            segments[other.Id] = uuid;
            that.UUID = uuid;
            return that;
        }

        public local.Leg convert(skyscanner.Leg other) {
            local.Leg that = new local.Leg() {
                UUID = other.Id,
                SegmentUUIDs = other.SegmentIds.Select(id => segments[id]).ToList(),
                Departure = other.Departure,
                Arrival = other.Arrival,
                Origin = places[other.OriginStation].Code,
                Destination = places[other.DestinationStation].Code,
                Duration = other.Duration
            };
            return that;
        }

        public local.Itinerary convert(skyscanner.Itinerary other) {
            local.Itinerary that = new local.Itinerary() {
                OutboundLegUUID = other.OutboundLegId,
                InboundLegUUID = other.InboundLegId,
                Price = other.PricingOptions.Select(o => o.Price).Min(),
                Agent = agents[other.PricingOptions.OrderBy(o => o.Price).First().Agents.First()].Name,
                Deeplink = other.PricingOptions.OrderBy(o => o.Price).First().DeeplinkUrl
            };
            string uuid = $"{that.OutboundLegUUID}/{that.InboundLegUUID}";
            that.UUID = uuid;
            return that;
        }
        
    }
}