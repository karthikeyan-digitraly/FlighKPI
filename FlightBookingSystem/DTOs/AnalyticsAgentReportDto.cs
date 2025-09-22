namespace FlightBookingSystem.DTOs
{
    public class AnalyticsAgentReportDto
    {
        public string AgentName { get; set; } = string.Empty;
        public int TotalBookings { get; set; }
        public decimal Revenue { get; set; }
        public int BlockedSeats { get; set; }
        public double ConversionRate { get; set; }
    }
}
