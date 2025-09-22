namespace FlightBookingSystem.Entities
{
    public class Agent
    {
        public int AgentID { get; set; }
        public int ProviderID { get; set; }
        public string AgentCode { get; set; } = string.Empty;
        public string? AgentName { get; set; }
        public string? Channel { get; set; }
        public string? Region { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Provider Provider { get; set; }
        public ICollection<Booking> Bookings { get; set; }
    }
}
