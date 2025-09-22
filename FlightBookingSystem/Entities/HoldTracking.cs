using System.ComponentModel.DataAnnotations;

namespace FlightBookingSystem.Entities
{
    public class HoldTracking
    {
        [Key]  
        public long HoldID { get; set; }

        public long BookingID { get; set; }
        public DateTime? HoldPlacedAt { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public bool? Converted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Booking Booking { get; set; }
    }
}
