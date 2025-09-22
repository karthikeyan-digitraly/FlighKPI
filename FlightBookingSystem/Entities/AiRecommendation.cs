using FlightBookingSystem.Enums;
using System.ComponentModel.DataAnnotations;

namespace FlightBookingSystem.Entities
{
    public class AiRecommendation
    {
        [Key]
        public long RecommendationID { get; set; }

        public long BookingID { get; set; }

        public RecommendationActionType ActionType { get; set; }

        public decimal? Confidence { get; set; }

        public RecommendationStatus Status { get; set; } = RecommendationStatus.Pending;

        public DateTime CreatedAt { get; set; }

        public Booking Booking { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Impact { get; set; } = "MED";
        public string SuggestedAction { get; set; } = string.Empty;
        public string PotentialGain { get; set; } = string.Empty;
        public string Context { get; set; } = "general";
    }
}
