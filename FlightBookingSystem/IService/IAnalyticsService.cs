using FlightBookingSystem.DTOs;

namespace FlightBookingSystem.IService
{
    public interface IAnalyticsService
    {
        Task<AnalyticsSummaryDto> GetSummaryAsync(
            DateTime startDate,
            DateTime endDate,
            string? channel = null,
            string? route = null
        );

        Task<Dictionary<string, AnalyticsSalesReportDto>> GetSalesReportAsync(
            DateTime startDate,
            DateTime endDate,
            string? channel = null,
            string? route = null
        );

        Task<List<AnalyticsAncillaryDto>> GetAncillariesAsync(
            DateTime startDate,
            DateTime endDate,
            string? channel = null,
            string? route = null
        );

        Task<DailyRevenueDto> GetDailyRevenueAsync(
            DateTime startDate,
            DateTime endDate,
            string? channel = null,
            string? route = null
        );

        Task<List<AnalyticsRecommendationDto>> GetRecommendationsAsync();
        Task<List<string>> GetChannelsAsync();
        Task<List<string>> GetRoutesAsync();


        Task<List<AnalyticsAgentReportDto>> GetAgentReportAsync(
    DateTime startDate, DateTime endDate, string? channel = null, string? route = null);
    }
}
