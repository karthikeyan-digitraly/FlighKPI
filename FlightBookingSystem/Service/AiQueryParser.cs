using Betalgo.Ranul.OpenAI.Interfaces;
using Betalgo.Ranul.OpenAI.ObjectModels.RequestModels;
using FlightBookingSystem.IService;

namespace FlightBookingSystem.Service
{
    public class AiQueryParser : IAiQueryParser
    {
        private readonly IOpenAIService _openAi;

        public AiQueryParser(IOpenAIService openAi)
        {
            _openAi = openAi;
        }

        public async Task<string> GenerateSqlQueryAsync(string userPrompt)
        {
            var system = ChatMessage.FromSystem(
    "You are a SQL generator for SQLite. " +
    "⚠️ IMPORTANT: Use only the exact table-prefixed column names provided. " +
    "Do not invent variations, do not add dots inside names (e.g., never Booking.BookingDate). " +
    "Do not alias column names, do not duplicate them. " +
    "Never use SELECT *. Always select only the columns relevant to the query. " +

    // Tables and exact column names
    "Tables and columns: " +
    "Booking (Booking_BookingID, Booking_ProviderID, Booking_ProviderBookingRef, Booking_PNR, Booking_AgentID, " +
    "Booking_ScheduleID, Booking_BookingDate, Booking_TravelDate, Booking_Status, Booking_FareAmount, Booking_Currency, Booking_PaxCount, " +
    "Booking_CreatedAt, Booking_UpdatedAt, Booking_ProviderName, Booking_AgentName, Booking_AgentCode), " +

    "Agent (Agent_AgentID, Agent_AgentName, Agent_AgentCode, Agent_AgentRegion, Agent_AgentChannel), " +
    "Provider (Provider_ProviderID, Provider_ProviderName), " +

    "Segment (Segment_SegmentID, Segment_BookingID, Segment_ProviderSegmentID, Segment_FlightNo, Segment_FromAirport, " +
    "Segment_ToAirport, Segment_CabinClass, Segment_SegmentStatus, Segment_SegmentFare, Segment_CreatedAt, Segment_UpdatedAt), " +

    "Ancillary (Ancillary_AncillaryID, Ancillary_BookingID, Ancillary_ProviderAncillaryID, Ancillary_Type, Ancillary_Amount, " +
    "Ancillary_Currency, Ancillary_CreatedAt, Ancillary_UpdatedAt), " +

    "Ticket (Ticket_TicketID, Ticket_BookingID, Ticket_TicketNumber, Ticket_PaxIndex, Ticket_IssueDate, " +
    "Ticket_TicketStatus, Ticket_CreatedAt, Ticket_UpdatedAt), " +

    "HoldTracking (HoldTracking_HoldID, HoldTracking_BookingID, HoldTracking_HoldPlacedAt, HoldTracking_ConfirmedAt, " +
    "HoldTracking_Converted, HoldTracking_CreatedAt, HoldTracking_UpdatedAt), " +

    // Relationships
    "Relationships: Booking_BookingID = Segment_BookingID, Booking_BookingID = Ancillary_BookingID, " +
    "Booking_BookingID = Ticket_BookingID, Booking_BookingID = HoldTracking_BookingID, " +
    "Booking_AgentID = Agent_AgentID, Booking_ProviderID = Provider_ProviderID. " +

    // Rules
    "Rules: " +
    "- Use SQLite's julianday() or datetime() functions for date arithmetic. " +
    "- Always use integers for enums (e.g., Segment_SegmentStatus = 1 instead of 'Cancelled'). " +
    "- When filtering by date, always use the correct column from the relevant table: " +
    "  Booking_BookingDate for bookings, Segment_CreatedAt for segments, Ticket_IssueDate for tickets, " +
    "HoldTracking_CreatedAt for hold tracking, Ancillary_CreatedAt for ancillaries. " +
    "- For top N per group queries, use ROW_NUMBER() OVER(PARTITION BY ...) instead of a global LIMIT. " +
    "- Always return only a single valid SQL query. No explanations."
);


            var user = ChatMessage.FromUser(userPrompt);

            var req = new ChatCompletionCreateRequest
            {
                Model = "gpt-4o-mini",
                Messages = new List<ChatMessage> { system, user },
                Temperature = 0
            };

            var completion = await _openAi.ChatCompletion.CreateCompletion(req);

            if (!completion.Successful || completion.Choices == null || completion.Choices.Count == 0)
                throw new InvalidOperationException($"OpenAI error: {completion.Error?.Message ?? "no choices returned"}");

            var sql = completion.Choices.First().Message.Content?.Trim() ?? string.Empty;

            if (sql.StartsWith("```"))
            {
                var firstLine = sql.IndexOf("\n") + 1;
                var lastTick = sql.LastIndexOf("```");
                sql = sql.Substring(firstLine, lastTick - firstLine).Trim();
            }

            return sql;
        }
    }
}
