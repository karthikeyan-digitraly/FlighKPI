namespace FlightBookingSystem.DTOs.CSVDtos
{
    public class AgentCsvDto
    {
        public int AgentID { get; set; }
        public int ProviderID { get; set; }
        public string? AgentCode { get; set; }    // <-- nullable
        public string? AgentName { get; set; }    // <-- nullable
        public string? Channel { get; set; }      // <-- nullable
        public string? Region { get; set; }       // <-- nullable
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? ProviderName { get; set; } // <-- nullable
    }

    public class ProviderCsvDto
    {
        public int ProviderID { get; set; }
        public string? ProviderName { get; set; }
        public string? ContactEmail { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class BookingCsvDto
    {
        public long BookingID { get; set; }
        public int ProviderID { get; set; }
        public string ProviderBookingRef { get; set; }
        public string? PNR { get; set; }
        public int? AgentID { get; set; }
        public int ScheduleID { get; set; }   

        public DateTime? BookingDate { get; set; }
        public DateTime? TravelDate { get; set; }
        public string? Status { get; set; }
        public decimal? FareAmount { get; set; }
        public string? Currency { get; set; }
        public int? PaxCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactName { get; set; }
        public string? ProviderName { get; set; }
        public string? AgentName { get; set; }
        public string? AgentCode { get; set; }
    }

    public class ScheduleCsvDto
    {
        public int ScheduleID { get; set; }
        public string AirlineCode { get; set; }
        public string FlightNo { get; set; }
        public string FromAirport { get; set; }
        public string ToAirport { get; set; }
        public DateTime TravelDate { get; set; }
        public int Capacity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class SegmentCsvDto
    {
        public long SegmentID { get; set; }
        public long BookingID { get; set; }
        public string? ProviderSegmentID { get; set; }
        public string? FlightNo { get; set; }
        public string? FromAirport { get; set; }
        public string? ToAirport { get; set; }
        public string? CabinClass { get; set; }
        public string? SegmentStatus { get; set; }
        public decimal? SegmentFare { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class TicketCsvDto
    {
        public long TicketID { get; set; }
        public long BookingID { get; set; }
        public string TicketNumber { get; set; }
        public int? PaxIndex { get; set; }
        public DateTime? IssueDate { get; set; }
        public string? TicketStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class AncillaryCsvDto
    {
        public long AncillaryID { get; set; }
        public long BookingID { get; set; }
        public string? ProviderAncillaryID { get; set; }
        public string? Type { get; set; }
        public decimal? Amount { get; set; }
        public string? Currency { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class HoldTrackingCsvDto
    {
        public long HoldID { get; set; }
        public long BookingID { get; set; }
        public DateTime? HoldPlacedAt { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public bool? Converted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class AiRecommendationCsvDto
    {
        public long RecommendationID { get; set; }
        public long BookingID { get; set; }
        public string ActionType { get; set; }
        public decimal? Confidence { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Title { get; set; }
        public string Impact { get; set; }
        public string SuggestedAction { get; set; }
        public string PotentialGain { get; set; }
        public string Context { get; set; }
    }
}
