using FlightBookingSystem.IService;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace FlightBookingSystem.Controllers
{
    [ApiController]
    [Route("api/chat")]
    public class ChatController : ControllerBase
    {
        private readonly IAiQueryParser _aiQueryParser;
        private readonly ICsvQueryService _csvQueryService;

        public ChatController(IAiQueryParser aiQueryParser, ICsvQueryService csvQueryService)
        {
            _aiQueryParser = aiQueryParser;
            _csvQueryService = csvQueryService;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] ChatRequest request)
        {
            string sqlQuery = await _aiQueryParser.GenerateSqlQueryAsync(request.Prompt);

            DataTable result = await _csvQueryService.ExecuteSqlOnCsvAsync(sqlQuery);

            var rows = result.AsEnumerable().Select(r =>
                result.Columns.Cast<DataColumn>().ToDictionary(c => c.ColumnName, c => r[c].ToString())
            );

            return Ok(new { prompt = request.Prompt, sql = sqlQuery, result = rows });
        }
    }

    public class ChatRequest
    {
        public string Prompt { get; set; } = string.Empty;
    }
}
