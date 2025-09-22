namespace FlightBookingSystem.Entities
{
    public class Provider
    {
        public int ProviderID { get; set; }
        public string? ProviderName { get; set; }
        public string? ContactEmail { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<Agent> Agents { get; set; }
        public ICollection<Booking> Bookings { get; set; }
    }
}
