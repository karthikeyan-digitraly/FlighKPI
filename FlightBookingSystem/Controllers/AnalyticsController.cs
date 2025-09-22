using FlightBookingSystem.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlightBookingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;

        public AnalyticsController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] string? channel = null,
            [FromQuery] string? route = null)
        {
            var result = await _analyticsService.GetSummaryAsync(startDate, endDate, channel, route);
            return Ok(result);
        }

        [HttpGet("sales-report")]
        public async Task<IActionResult> GetSalesReport(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] string? channel = null,
            [FromQuery] string? route = null)
        {
            var result = await _analyticsService.GetSalesReportAsync(startDate, endDate, channel, route);
            return Ok(result);
        }

        [HttpGet("ancillaries")]
        public async Task<IActionResult> GetAncillaries(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] string? channel = null,
            [FromQuery] string? route = null)
        {
            var result = await _analyticsService.GetAncillariesAsync(startDate, endDate, channel, route);
            return Ok(result);
        }

        [HttpGet("sales/daily")]
        public async Task<IActionResult> GetDailyRevenue(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] string? channel = null,
            [FromQuery] string? route = null)
        {
            var data = await _analyticsService.GetDailyRevenueAsync(startDate, endDate, channel, route);
            return Ok(data);
        }

        [HttpGet("recommendations")]
        public async Task<IActionResult> GetRecommendations()
        {
            var result = await _analyticsService.GetRecommendationsAsync();
            return Ok(result);
        }

        [HttpGet("channels")]
        public async Task<IActionResult> GetChannels()
        {
            var channels = await _analyticsService.GetChannelsAsync();
            return Ok(channels);
        }

        [HttpGet("routes")]
        public async Task<IActionResult> GetRoutes()
        {
            var routes = await _analyticsService.GetRoutesAsync();
            return Ok(routes);
        }

        [HttpGet("agents")]
        public async Task<IActionResult> GetAgentReport(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] string? channel = null,
            [FromQuery] string? route = null)
        {
            var data = await _analyticsService.GetAgentReportAsync(startDate, endDate, channel, route);
            return Ok(data);
        }
    }
}
