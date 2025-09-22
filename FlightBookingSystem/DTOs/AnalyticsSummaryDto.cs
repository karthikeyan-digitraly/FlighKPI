namespace FlightBookingSystem.DTOs
{
    public class AnalyticsSummaryDto
    {
        public int TotalSegments { get; set; }
        public decimal TotalRevenue { get; set; }
        public double SegmentsChangePercent { get; set; }
        public double RevenueChangePercent { get; set; }
        public Dictionary<string, double> AncillaryRates { get; set; }
    }
}
