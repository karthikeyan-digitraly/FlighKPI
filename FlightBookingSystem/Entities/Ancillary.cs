using FlightBookingSystem.Enums;

namespace FlightBookingSystem.Entities
{
    public class Ancillary
    {
        public long AncillaryID { get; set; }
        public long BookingID { get; set; }
        public string? ProviderAncillaryID { get; set; }
        public AncillaryType? Type { get; set; }
        public decimal? Amount { get; set; }
        public string? Currency { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Booking Booking { get; set; }
    }
}
