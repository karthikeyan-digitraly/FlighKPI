namespace FlightBookingSystem.DTOs
{
    public class DailyRevenueDto
    {
        public Dictionary<DateTime, decimal> DailyRevenue { get; set; } = new();
        public decimal TotalRevenue { get; set; }
    }
}
