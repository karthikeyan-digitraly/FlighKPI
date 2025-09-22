using FlightBookingSystem.Enums;

namespace FlightBookingSystem.Entities
{
    public class Ticket
    {
        public long TicketID { get; set; }
        public long BookingID { get; set; }
        public string TicketNumber { get; set; } = string.Empty;
        public int? PaxIndex { get; set; }
        public DateTime? IssueDate { get; set; }
        public TicketStatus? TicketStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Booking Booking { get; set; }
    }
}
