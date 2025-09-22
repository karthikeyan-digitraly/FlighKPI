using FlightBookingSystem.Enums;

namespace FlightBookingSystem.Entities
{
    public class Segment
    {
        public long SegmentID { get; set; }
        public long BookingID { get; set; }
        public string? ProviderSegmentID { get; set; }
        public string? FlightNo { get; set; }
        public string? FromAirport { get; set; }
        public string? ToAirport { get; set; }
        public string? CabinClass { get; set; }
        public SegmentStatus? SegmentStatus { get; set; }
        public decimal? SegmentFare { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Booking Booking { get; set; }
    }
}
