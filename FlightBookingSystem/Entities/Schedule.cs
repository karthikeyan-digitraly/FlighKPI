namespace FlightBookingSystem.Entities
{
    public class Schedule
    {
        public int ScheduleID { get; set; }
        public string AirlineCode { get; set; } = string.Empty;
        public string FlightNo { get; set; } = string.Empty;
        public string FromAirport { get; set; } = string.Empty;
        public string ToAirport { get; set; } = string.Empty;
        public DateTime TravelDate { get; set; }
        public int Capacity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
