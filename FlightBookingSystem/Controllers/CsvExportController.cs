using Microsoft.AspNetCore.Mvc;
using FlightBookingSystem.IService;

namespace FlightBookingSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CsvExportController : ControllerBase
    {
        private readonly ICsvExportService _csvExportService;
        private readonly ILogger<CsvExportController> _logger;

        public CsvExportController(ICsvExportService csvExportService, ILogger<CsvExportController> logger)
        {
            _csvExportService = csvExportService;
            _logger = logger;
        }

        [HttpPost("export-all")]
        public async Task<IActionResult> ExportAllAsync()
        {
            try
            {
                await _csvExportService.ExportAllAsync();
                _logger.LogInformation("CSV export completed successfully.");
                return Ok(new { message = "CSV export completed successfully.", path = "CSV folder in application root." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while exporting CSV.");
                return StatusCode(500, new { message = "Error occurred while exporting CSV.", error = ex.Message });
            }
        }
    }
}
