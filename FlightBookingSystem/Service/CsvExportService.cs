using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CsvHelper;
using FlightBookingSystem.Data;
using FlightBookingSystem.DTOs.CSVDtos;
using System.Collections.Generic;
using System.Linq;
using FlightBookingSystem.IService;

namespace FlightBookingSystem.Services
{
    public class CsvExportService : ICsvExportService
    {
        private readonly AppDbContext _db;
        private readonly string _csvPath;

        public CsvExportService(AppDbContext db)
        {
            _db = db;

            // Use current working directory (project root when running in IDE)
            _csvPath = Path.Combine(Directory.GetCurrentDirectory(), "CSV");

            if (!Directory.Exists(_csvPath))
                Directory.CreateDirectory(_csvPath);

            // Optional: Log folder location
            Console.WriteLine($"CSV files will be exported to: {_csvPath}");
        }

        public async Task ExportAllAsync()
        {
            await ExportAgentsAsync();
            await ExportProvidersAsync();
            await ExportBookingsAsync();
            await ExportSchedulesAsync();
            await ExportSegmentsAsync();
            await ExportTicketsAsync();
            await ExportAncillariesAsync();
            await ExportHoldTrackingsAsync();
            await ExportAiRecommendationsAsync();
        }

        private async Task ExportAgentsAsync()
        {
            var agents = await _db.Agents
                .Include(a => a.Provider)
                .AsNoTracking()
                .Select(a => new AgentCsvDto
                {
                    AgentID = a.AgentID,
                    ProviderID = a.ProviderID,
                    AgentCode = a.AgentCode != null ? a.AgentCode : string.Empty,
                    AgentName = a.AgentName != null ? a.AgentName : string.Empty,
                    Channel = a.Channel != null ? a.Channel : string.Empty,
                    Region = a.Region != null ? a.Region : string.Empty,
                    CreatedAt = a.CreatedAt,
                    UpdatedAt = a.UpdatedAt,
                    ProviderName = a.Provider != null && a.Provider.ProviderName != null ? a.Provider.ProviderName : string.Empty
                })
                .ToListAsync();

            await ExportAsync("Agent", agents);
        }

        private async Task ExportProvidersAsync()
        {
            var providers = await _db.Providers
                .AsNoTracking()
                .Select(p => new ProviderCsvDto
                {
                    ProviderID = p.ProviderID,
                    ProviderName = p.ProviderName != null ? p.ProviderName : string.Empty,
                    ContactEmail = p.ContactEmail != null ? p.ContactEmail : string.Empty,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync();

            await ExportAsync("Provider", providers);
        }

        private async Task ExportBookingsAsync()
        {
            var bookings = await _db.Bookings
                .Include(b => b.Provider)
                .Include(b => b.Agent)
                .AsNoTracking()
                .Select(b => new BookingCsvDto
                {
                    BookingID = b.BookingID,
                    ProviderID = b.ProviderID,
                    ProviderBookingRef = b.ProviderBookingRef != null ? b.ProviderBookingRef : string.Empty,
                    PNR = b.PNR != null ? b.PNR : string.Empty,
                    AgentID = b.AgentID,
                    ScheduleID = b.ScheduleID,
                    BookingDate = b.BookingDate,
                    TravelDate = b.TravelDate,
                    Status = b.Status != null ? b.Status.ToString() : string.Empty,
                    FareAmount = b.FareAmount,
                    Currency = b.Currency != null ? b.Currency : string.Empty,
                    PaxCount = b.PaxCount,
                    CreatedAt = b.CreatedAt,
                    UpdatedAt = b.UpdatedAt,
                    ContactEmail = b.ContactEmail != null ? b.ContactEmail : string.Empty,
                    ContactName = b.ContactName != null ? b.ContactName : string.Empty,
                    ProviderName = b.Provider != null && b.Provider.ProviderName != null ? b.Provider.ProviderName : string.Empty,
                    AgentName = b.Agent != null && b.Agent.AgentName != null ? b.Agent.AgentName : string.Empty,
                    AgentCode = b.Agent != null && b.Agent.AgentCode != null ? b.Agent.AgentCode : string.Empty
                })
                .ToListAsync();

            await ExportAsync("Booking", bookings);
        }

        private async Task ExportSchedulesAsync()
        {
            var schedules = await _db.Schedules
                .AsNoTracking()
                .Select(s => new ScheduleCsvDto
                {
                    ScheduleID = s.ScheduleID,
                    AirlineCode = s.AirlineCode != null ? s.AirlineCode : string.Empty,
                    FlightNo = s.FlightNo != null ? s.FlightNo : string.Empty,
                    FromAirport = s.FromAirport != null ? s.FromAirport : string.Empty,
                    ToAirport = s.ToAirport != null ? s.ToAirport : string.Empty,
                    TravelDate = s.TravelDate,
                    Capacity = s.Capacity,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt
                })
                .ToListAsync();

            await ExportAsync("Schedule", schedules);
        }

        private async Task ExportSegmentsAsync()
        {
            var segments = await _db.Segments
                .AsNoTracking()
                .Select(s => new SegmentCsvDto
                {
                    SegmentID = s.SegmentID,
                    BookingID = s.BookingID,
                    ProviderSegmentID = s.ProviderSegmentID != null ? s.ProviderSegmentID : string.Empty,
                    FlightNo = s.FlightNo != null ? s.FlightNo : string.Empty,
                    FromAirport = s.FromAirport != null ? s.FromAirport : string.Empty,
                    ToAirport = s.ToAirport != null ? s.ToAirport : string.Empty,
                    CabinClass = s.CabinClass != null ? s.CabinClass : string.Empty,
                    SegmentStatus = s.SegmentStatus != null ? s.SegmentStatus.ToString() : string.Empty,
                    SegmentFare = s.SegmentFare,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt
                })
                .ToListAsync();

            await ExportAsync("Segment", segments);
        }

        private async Task ExportTicketsAsync()
        {
            var tickets = await _db.Tickets
                .AsNoTracking()
                .Select(t => new TicketCsvDto
                {
                    TicketID = t.TicketID,
                    BookingID = t.BookingID,
                    TicketNumber = t.TicketNumber != null ? t.TicketNumber : string.Empty,
                    PaxIndex = t.PaxIndex,
                    IssueDate = t.IssueDate,
                    TicketStatus = t.TicketStatus != null ? t.TicketStatus.ToString() : string.Empty,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                })
                .ToListAsync();

            await ExportAsync("Ticket", tickets);
        }

        private async Task ExportAncillariesAsync()
        {
            var ancillaries = await _db.Ancillaries
                .AsNoTracking()
                .Select(a => new AncillaryCsvDto
                {
                    AncillaryID = a.AncillaryID,
                    BookingID = a.BookingID,
                    ProviderAncillaryID = a.ProviderAncillaryID != null ? a.ProviderAncillaryID : string.Empty,
                    Type = a.Type != null ? a.Type.ToString() : string.Empty,
                    Amount = a.Amount,
                    Currency = a.Currency != null ? a.Currency : string.Empty,
                    CreatedAt = a.CreatedAt,
                    UpdatedAt = a.UpdatedAt
                })
                .ToListAsync();

            await ExportAsync("Ancillary", ancillaries);
        }

        private async Task ExportHoldTrackingsAsync()
        {
            var holds = await _db.HoldTrackings
                .AsNoTracking()
                .Select(h => new HoldTrackingCsvDto
                {
                    HoldID = h.HoldID,
                    BookingID = h.BookingID,
                    HoldPlacedAt = h.HoldPlacedAt,
                    ConfirmedAt = h.ConfirmedAt,
                    Converted = h.Converted,
                    CreatedAt = h.CreatedAt,
                    UpdatedAt = h.UpdatedAt
                })
                .ToListAsync();

            await ExportAsync("HoldTracking", holds);
        }

        private async Task ExportAiRecommendationsAsync()
        {
            var recs = await _db.AiRecommendations
                .AsNoTracking()
                .Select(r => new AiRecommendationCsvDto
                {
                    RecommendationID = r.RecommendationID,
                    BookingID = r.BookingID,
                    ActionType = r.ActionType != null ? r.ActionType.ToString() : string.Empty,
                    Confidence = r.Confidence,
                    Status = r.Status != null ? r.Status.ToString() : string.Empty,
                    CreatedAt = r.CreatedAt,
                    Title = r.Title != null ? r.Title : string.Empty,
                    Impact = r.Impact != null ? r.Impact : string.Empty,
                    SuggestedAction = r.SuggestedAction != null ? r.SuggestedAction : string.Empty,
                    PotentialGain = r.PotentialGain != null ? r.PotentialGain : string.Empty,
                    Context = r.Context != null ? r.Context : string.Empty
                })
                .ToListAsync();

            await ExportAsync("AiRecommendation", recs);
        }

        private async Task ExportAsync<T>(string fileName, IEnumerable<T> data)
        {
            var file = Path.Combine(_csvPath, $"{fileName}.csv");

            await using var writer = new StreamWriter(file);
            await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            await csv.WriteRecordsAsync(data);

            // Optional: Log each file creation
            Console.WriteLine($"Exported {fileName} to {file}");
        }
    }
}
