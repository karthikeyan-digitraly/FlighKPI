using CsvHelper;
using FlightBookingSystem.IService;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Globalization;

namespace FlightBookingSystem.Service
{
    public class CsvQueryService : ICsvQueryService
    {
        private readonly string _csvPath;

        private static readonly Dictionary<(string Table, string Column), string> _columnMappings =
    new()
    {
        { ("Agent", "Region"), "AgentRegion" },
        { ("Agent", "Channel"), "AgentChannel" }
    };
        public CsvQueryService(IConfiguration config)
        {
            _csvPath = Path.Combine(AppContext.BaseDirectory, "CSV");
            if (!Directory.Exists(_csvPath))
                Directory.CreateDirectory(_csvPath);
        }

        public async Task<DataTable> ExecuteSqlOnCsvAsync(string sqlQuery)
        {
            var dt = new DataTable();

            await Task.Run(() =>
            {
                using var conn = new SqliteConnection("DataSource=:memory:");
                conn.Open();

                foreach (var file in Directory.GetFiles(_csvPath, "*.csv"))
                {
                    var tableName = Path.GetFileNameWithoutExtension(file);

                    using var reader = new StreamReader(file);
                    using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

                    csv.Read();
                    csv.ReadHeader();
                    var headers = csv.HeaderRecord;

                    var existingColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                    using var createCmd = conn.CreateCommand();
                    var columnsDef = headers.Select(h =>
                    {
                        var mappedHeader = _columnMappings.TryGetValue((tableName, h), out var mapped)
                            ? mapped
                            : h;

                        var colName = $"{tableName}_{mappedHeader}";

                        // Ensure uniqueness
                        if (existingColumns.Contains(colName))
                        {
                            int suffix = 1;
                            while (existingColumns.Contains($"{colName}_{suffix}"))
                                suffix++;
                            colName = $"{colName}_{suffix}";
                        }

                        existingColumns.Add(colName);

                        if (IsEnumColumn(tableName, h)) return $"{colName} INTEGER";
                        if (IsNumericColumn(tableName, h)) return $"{colName} REAL";
                        if (h.EndsWith("Date", StringComparison.OrdinalIgnoreCase) ||
                            h.EndsWith("CreatedAt", StringComparison.OrdinalIgnoreCase) ||
                            h.EndsWith("UpdatedAt", StringComparison.OrdinalIgnoreCase))
                            return $"{colName} TEXT";

                        return $"{colName} TEXT";
                    });

                    createCmd.CommandText = $"CREATE TABLE {tableName} ({string.Join(", ", columnsDef)});";
                    createCmd.ExecuteNonQuery();

                    while (csv.Read())
                    {
                        var values = headers.Select(h =>
                        {
                            var mappedHeader = _columnMappings.TryGetValue((tableName, h), out var mapped)
                                ? mapped
                                : h;

                            var colName = $"{tableName}_{mappedHeader}";
                            if (!existingColumns.Contains(colName))
                                colName = existingColumns.First(c => c.StartsWith(colName));

                            var field = csv.TryGetField<string>(h, out var value) ? value : "";

                            if (IsEnumColumn(tableName, h))
                                field = MapEnumStringToInt(tableName, field).ToString();

                            if (h.EndsWith("Date", StringComparison.OrdinalIgnoreCase) ||
                                h.EndsWith("CreatedAt", StringComparison.OrdinalIgnoreCase) ||
                                h.EndsWith("UpdatedAt", StringComparison.OrdinalIgnoreCase))
                            {
                                if (DateTime.TryParse(field, out var dtValue))
                                    field = dtValue.ToString("yyyy-MM-dd");
                            }

                            return $"'{field.Replace("'", "''")}'";
                        });

                        using var insertCmd = conn.CreateCommand();
                        insertCmd.CommandText = $"INSERT INTO {tableName} VALUES ({string.Join(",", values)});";
                        insertCmd.ExecuteNonQuery();
                    }
                }

                using var queryCmd = conn.CreateCommand();
                queryCmd.CommandText = sqlQuery;

                using var readerResult = queryCmd.ExecuteReader();
                dt.Load(readerResult);
            });

            return dt;
        }

        private static bool IsEnumColumn(string tableName, string columnName) =>
            (tableName, columnName) switch
            {
                ("Booking", "Status") => true,
                ("Segment", "SegmentStatus") => true,
                ("Ticket", "TicketStatus") => true,
                ("Ancillary", "Type") => true,
                _ => false
            };

        private static bool IsNumericColumn(string tableName, string columnName) =>
            (tableName, columnName) switch
            {
                ("Booking", "FareAmount") => true,
                ("Booking", "PaxCount") => true,
                ("Segment", "SegmentFare") => true,
                ("Ancillary", "Amount") => true,
                _ => false
            };

        private static int MapEnumStringToInt(string tableName, string value) =>
            (tableName, value) switch
            {
                ("Booking", "Hold") => 0,
                ("Booking", "Confirmed") => 1,
                ("Booking", "Cancelled") => 2,

                ("Segment", "Scheduled") => 0,
                ("Segment", "Cancelled") => 1,
                ("Segment", "Completed") => 2,

                ("Ticket", "Issued") => 0,
                ("Ticket", "Voided") => 1,
                ("Ticket", "Refunded") => 2,

                ("Ancillary", "Unknown") => 0,
                ("Ancillary", "Baggage") => 1,
                ("Ancillary", "SeatSelection") => 2,
                ("Ancillary", "Meal") => 3,
                ("Ancillary", "Wifi") => 4,
                ("Ancillary", "Insurance") => 5,
                ("Ancillary", "Lounge") => 6,
                _ => 0
            };
    }
}
