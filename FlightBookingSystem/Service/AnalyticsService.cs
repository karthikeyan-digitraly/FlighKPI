using FlightBookingSystem.Data;
using FlightBookingSystem.DTOs;
using FlightBookingSystem.Entities;
using FlightBookingSystem.IService;
using Microsoft.EntityFrameworkCore;

namespace FlightBookingSystem.Service
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly AppDbContext _context;

        public AnalyticsService(AppDbContext context)
        {
            _context = context;
        }

        private IQueryable<Booking> ApplyFilters(IQueryable<Booking> query, string? channel, string? route)
        {
            if (!string.IsNullOrEmpty(channel) && channel != "All")
                query = query.Where(b => b.Agent != null && b.Agent.Channel == channel);

            if (!string.IsNullOrEmpty(route) && route != "All")
            {
                var parts = route.Split('-');
                if (parts.Length == 2)
                {
                    var from = parts[0];
                    var to = parts[1];
                    query = query.Where(b => b.Segments.Any(s => s.FromAirport == from && s.ToAirport == to));
                }
            }

            return query;
        }

        public async Task<AnalyticsSummaryDto> GetSummaryAsync(
            DateTime startDate,
            DateTime endDate,
            string? channel,
            string? route )
        {
            var bookingsQuery = _context.Bookings
                .Include(b => b.Segments)
                .Include(b => b.Ancillaries)
                .Include(b => b.Agent)
                .Where(b => b.BookingDate >= startDate && b.BookingDate <= endDate);

            bookingsQuery = ApplyFilters(bookingsQuery, channel, route);
            var bookings = await bookingsQuery.ToListAsync();

            var periodDays = (endDate - startDate).Days;
            var prevStart = startDate.AddDays(-periodDays);
            var prevEnd = startDate.AddDays(-1);

            var prevBookingsQuery = _context.Bookings
                .Include(b => b.Segments)
                .Include(b => b.Ancillaries)
                .Include(b => b.Agent)
                .Where(b => b.BookingDate >= prevStart && b.BookingDate <= prevEnd);

            prevBookingsQuery = ApplyFilters(prevBookingsQuery, channel, route);
            var prevBookings = await prevBookingsQuery.ToListAsync();

            var totalSegments = bookings.Sum(b => b.Segments.Count);
            var totalRevenue = bookings.Sum(b => (b.FareAmount ?? 0) + b.Ancillaries.Sum(a => a.Amount.GetValueOrDefault()));

            var prevSegments = prevBookings.Sum(b => b.Segments.Count);
            var prevRevenue = prevBookings.Sum(b => (b.FareAmount ?? 0) + b.Ancillaries.Sum(a => a.Amount.GetValueOrDefault()));

            decimal segmentsChangePercent = prevSegments == 0 ? 100 :
                ((decimal)(totalSegments - prevSegments) / prevSegments) * 100;

            decimal revenueChangePercent = prevRevenue == 0 ? 100 :
                ((totalRevenue - prevRevenue) / prevRevenue) * 100;

            var ancillaryGroups = bookings
                .SelectMany(b => b.Ancillaries)
                .GroupBy(a => a.Type.HasValue ? a.Type.Value.ToString() : "Unknown")
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(x => x.Amount.GetValueOrDefault())
                );

            var ancillaryRates = ancillaryGroups.ToDictionary(
                g => g.Key,
                g => (double)(g.Value / (ancillaryGroups.Values.Sum() == 0 ? 1 : ancillaryGroups.Values.Sum()) * 100)
            );

            return new AnalyticsSummaryDto
            {
                TotalSegments = totalSegments,
                TotalRevenue = totalRevenue,
                SegmentsChangePercent = (double)segmentsChangePercent,
                RevenueChangePercent = (double)revenueChangePercent,
                AncillaryRates = ancillaryRates
            };
        }

        public async Task<Dictionary<string, AnalyticsSalesReportDto>> GetSalesReportAsync(
            DateTime startDate,
            DateTime endDate,
            string? channel = null,
            string? route = null)
        {
            var bookingsQuery = _context.Bookings
                .Include(b => b.Agent)
                .Include(b => b.Ancillaries)
                .Where(b => b.BookingDate >= startDate && b.BookingDate <= endDate);

            bookingsQuery = ApplyFilters(bookingsQuery, channel, route);
            var bookings = await bookingsQuery.ToListAsync();

            var grouped = bookings
                .Where(b => b.Agent != null)
                .GroupBy(b => b.Agent.Channel ?? "Unknown")
                .ToDictionary(
                    g => g.Key,
                    g => new
                    {
                        Count = g.Count(),
                        Revenue = g.Sum(b => (b.FareAmount ?? 0) + (b.Ancillaries?.Sum(a => a.Amount.GetValueOrDefault()) ?? 0))
                    }
                );

            var totalRevenue = grouped.Values.Sum(x => x.Revenue);

            return grouped.ToDictionary(
                g => g.Key,
                g => new AnalyticsSalesReportDto
                {
                    Count = g.Value.Count,
                    Revenue = g.Value.Revenue,
                    Percentage = totalRevenue == 0 ? 0 : (double)(g.Value.Revenue / totalRevenue * 100)
                }
            );
        }

        public async Task<List<AnalyticsAncillaryDto>> GetAncillariesAsync(
            DateTime startDate,
            DateTime endDate,
            string? channel = null,
            string? route = null)
        {
            var ancillariesQuery = _context.Ancillaries
                .Include(a => a.Booking)
                .ThenInclude(b => b.Segments)
                .Include(a => a.Booking.Agent)
                .Where(a => a.Booking.BookingDate >= startDate && a.Booking.BookingDate <= endDate);

            if (!string.IsNullOrEmpty(channel) && channel != "All")
                ancillariesQuery = ancillariesQuery.Where(a => a.Booking.Agent != null && a.Booking.Agent.Channel == channel);

            if (!string.IsNullOrEmpty(route) && route != "All")
            {
                var parts = route.Split('-');
                if (parts.Length == 2)
                {
                    var from = parts[0];
                    var to = parts[1];
                    ancillariesQuery = ancillariesQuery.Where(a => a.Booking.Segments.Any(s => s.FromAirport == from && s.ToAirport == to));
                }
            }

            var ancillaries = await ancillariesQuery.ToListAsync();
            var total = ancillaries.Sum(a => a.Amount.GetValueOrDefault());

            return ancillaries
                .GroupBy(a => a.Type.HasValue ? a.Type.Value.ToString() : "Unknown")
                .Select(g => new AnalyticsAncillaryDto
                {
                    Type = g.Key,
                    Amount = g.Sum(x => x.Amount.GetValueOrDefault()),
                    Percentage = total == 0 ? 0 : (double)(g.Sum(x => x.Amount.GetValueOrDefault()) / total * 100)
                })
                .ToList();
        }

        public async Task<DailyRevenueDto> GetDailyRevenueAsync(
            DateTime startDate,
            DateTime endDate,
            string? channel = null,
            string? route = null)
        {
            var bookingsQuery = _context.Bookings
                .Include(b => b.Ancillaries)
                .Include(b => b.Segments)
                .Include(b => b.Agent)
                .Where(b => b.BookingDate >= startDate && b.BookingDate <= endDate);

            bookingsQuery = ApplyFilters(bookingsQuery, channel, route);
            var bookings = await bookingsQuery.ToListAsync();

            var dailyRevenue = bookings
                .GroupBy(b => b.BookingDate)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(b => (b.FareAmount ?? 0) + (b.Ancillaries?.Sum(a => a.Amount.GetValueOrDefault()) ?? 0))
                );

            var resultDict = new Dictionary<DateTime, decimal>();
            for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
            {
                resultDict[date] = dailyRevenue.ContainsKey(date) ? dailyRevenue[date] : 0m;
            }

            var totalRevenue = resultDict.Values.Sum();

            return new DailyRevenueDto
            {
                DailyRevenue = resultDict,
                TotalRevenue = totalRevenue
            };
        }

        public async Task<List<AnalyticsRecommendationDto>> GetRecommendationsAsync()
        {
            return await _context.AiRecommendations
                .Select(r => new AnalyticsRecommendationDto
                {
                    Title = r.Title,
                    Impact = r.Impact,
                    SuggestedAction = r.SuggestedAction,
                    PotentialGain = r.PotentialGain
                })
                .ToListAsync();
        }

        public async Task<List<string>> GetChannelsAsync()
        {
            return await _context.Agents
                .Where(a => !string.IsNullOrEmpty(a.Channel))
                .Select(a => a.Channel!)
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<string>> GetRoutesAsync()
        {
            return await _context.Segments
                .Select(s => s.FromAirport + "-" + s.ToAirport)
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<AnalyticsAgentReportDto>> GetAgentReportAsync(
            DateTime startDate, DateTime endDate, string? channel = null, string? route = null)
        {
            var agentsQuery = _context.Agents
                .Include(a => a.Bookings)
                    .ThenInclude(b => b.Segments)
                .Include(a => a.Bookings)
                    .ThenInclude(b => b.Ancillaries)
                .Include(a => a.Bookings)
                    .ThenInclude(b => b.HoldTrackings)
                .AsQueryable();

            if (!string.IsNullOrEmpty(channel) && channel != "All")
                agentsQuery = agentsQuery.Where(a => a.Channel == channel);

            var agents = await agentsQuery.ToListAsync();

            var report = agents.Select(a =>
            {
                var filteredBookings = a.Bookings
                    .Where(b => b.BookingDate >= startDate && b.BookingDate <= endDate);

                if (!string.IsNullOrEmpty(route) && route != "All")
                {
                    var parts = route.Split('-');
                    if (parts.Length == 2)
                    {
                        var from = parts[0];
                        var to = parts[1];
                        filteredBookings = filteredBookings
                            .Where(b => b.Segments.Any(s => s.FromAirport == from && s.ToAirport == to));
                    }
                }

                var totalBookings = filteredBookings.Count();
                var revenue = filteredBookings.Sum(b => (b.FareAmount ?? 0) + b.Ancillaries.Sum(a => a.Amount.GetValueOrDefault()));
                var blockedSeats = filteredBookings
                    .SelectMany(b => b.HoldTrackings)
                    .Count(ht => ht.ConfirmedAt == null);
                var totalHolds = filteredBookings
                    .SelectMany(b => b.HoldTrackings)
                    .Count();
                var convertedHolds = totalHolds - blockedSeats;
                var conversionRate = totalHolds == 0 ? 0 : (convertedHolds * 100.0 / totalHolds);

                return new AnalyticsAgentReportDto
                {
                    AgentName = a.AgentName ?? a.AgentCode,
                    TotalBookings = totalBookings,
                    Revenue = revenue,
                    BlockedSeats = blockedSeats,
                    ConversionRate = conversionRate
                };
            }).ToList();

            return report;
        }
    }
}
