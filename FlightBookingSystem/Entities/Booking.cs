using FlightBookingSystem.Enums;
using System.Net.Sockets;

namespace FlightBookingSystem.Entities
{
    public class Booking
    {
        public long BookingID { get; set; }
        public int ProviderID { get; set; }
        public string ProviderBookingRef { get; set; } = string.Empty;
        public string? PNR { get; set; }
        public int? AgentID { get; set; }
        public DateTime? BookingDate { get; set; }
        public DateTime? TravelDate { get; set; }
        public BookingStatus? Status { get; set; }
        public decimal? FareAmount { get; set; }
        public string? Currency { get; set; }
        public int? PaxCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactName { get; set; }
        public int ScheduleID { get; set; }
        public Provider Provider { get; set; }
        public Agent? Agent { get; set; }
        public Schedule? Schedule { get; set; }


        public ICollection<Segment> Segments { get; set; }
        public ICollection<Ancillary> Ancillaries { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
        public ICollection<HoldTracking> HoldTrackings { get; set; }
        public ICollection<AiRecommendation> AiRecommendations { get; set; }
        public ICollection<Schedule> Schedules { get; set; }


    }
}
