using FluentMigrator;

namespace FlightBookingSystem.Data
{
    [Migration(202509180001)]
    public class CreateKpiBookingTables : Migration
    {
        public override void Up()
        {
            Create.Table("Provider")
                .WithColumn("ProviderID").AsInt32().PrimaryKey().Identity()
                .WithColumn("ProviderName").AsString(255).Nullable()
                .WithColumn("ContactEmail").AsString(255).Nullable()
                .WithColumn("CreatedAt").AsDateTime().NotNullable();

            Create.Table("Agent")
                .WithColumn("AgentID").AsInt32().PrimaryKey().Identity()
                .WithColumn("ProviderID").AsInt32().NotNullable()
                .WithColumn("AgentCode").AsString(50).NotNullable()
                .WithColumn("AgentName").AsString(255).Nullable()
                .WithColumn("Channel").AsString(50).Nullable()
                .WithColumn("Region").AsString(50).Nullable()
                .WithColumn("CreatedAt").AsDateTime().NotNullable()
                .WithColumn("UpdatedAt").AsDateTime().NotNullable();

            Create.ForeignKey("FK_Agent_Provider")
                .FromTable("Agent").ForeignColumn("ProviderID")
                .ToTable("Provider").PrimaryColumn("ProviderID");

            Create.Table("Schedule")
                .WithColumn("ScheduleID").AsInt32().PrimaryKey().Identity()
                .WithColumn("AirlineCode").AsString(10).NotNullable()
                .WithColumn("FlightNo").AsString(20).NotNullable()
                .WithColumn("FromAirport").AsString(10).NotNullable()
                .WithColumn("ToAirport").AsString(10).NotNullable()
                .WithColumn("TravelDate").AsDateTime().NotNullable()
                .WithColumn("Capacity").AsInt32().Nullable()
                .WithColumn("CreatedAt").AsDateTime().NotNullable()
                .WithColumn("UpdatedAt").AsDateTime().NotNullable();

            Create.Table("Booking")
                .WithColumn("BookingID").AsInt64().PrimaryKey().Identity()
                .WithColumn("ProviderID").AsInt32().NotNullable()
                .WithColumn("ProviderBookingRef").AsString(255).NotNullable()
                .WithColumn("PNR").AsString(50).Nullable()
                .WithColumn("AgentID").AsInt32().Nullable()
                .WithColumn("ScheduleID").AsInt32().Nullable()
                .WithColumn("BookingDate").AsDateTime().Nullable()
                .WithColumn("TravelDate").AsDateTime().Nullable()
                .WithColumn("Status").AsInt32().Nullable()
                .WithColumn("FareAmount").AsDecimal().Nullable()
                .WithColumn("Currency").AsString(10).Nullable()
                .WithColumn("PaxCount").AsInt32().Nullable()
                .WithColumn("CreatedAt").AsDateTime().NotNullable()
                .WithColumn("UpdatedAt").AsDateTime().NotNullable()
                .WithColumn("ContactEmail").AsString(255).Nullable()
                .WithColumn("ContactName").AsString(255).Nullable();

            Create.ForeignKey("FK_Booking_Agent")
                .FromTable("Booking").ForeignColumn("AgentID")
                .ToTable("Agent").PrimaryColumn("AgentID")
                .OnDeleteOrUpdate(System.Data.Rule.None);

            Create.ForeignKey("FK_Booking_Provider")
                .FromTable("Booking").ForeignColumn("ProviderID")
                .ToTable("Provider").PrimaryColumn("ProviderID")
                .OnDeleteOrUpdate(System.Data.Rule.None);

            Create.ForeignKey("FK_Booking_Schedule")
                .FromTable("Booking").ForeignColumn("ScheduleID")
                .ToTable("Schedule").PrimaryColumn("ScheduleID")
                .OnDeleteOrUpdate(System.Data.Rule.None);

            Create.Table("Ancillary")
                .WithColumn("AncillaryID").AsInt64().PrimaryKey().Identity()
                .WithColumn("BookingID").AsInt64().NotNullable()
                .WithColumn("ProviderAncillaryID").AsString(50).Nullable()
                .WithColumn("Type").AsString(50).Nullable()
                .WithColumn("Amount").AsDecimal().Nullable()
                .WithColumn("Currency").AsString(10).Nullable()
                .WithColumn("CreatedAt").AsDateTime().NotNullable()
                .WithColumn("UpdatedAt").AsDateTime().NotNullable();

            Create.ForeignKey("FK_Ancillary_Booking")
                .FromTable("Ancillary").ForeignColumn("BookingID")
                .ToTable("Booking").PrimaryColumn("BookingID");

            Create.Table("Segment")
                .WithColumn("SegmentID").AsInt64().PrimaryKey().Identity()
                .WithColumn("BookingID").AsInt64().NotNullable()
                .WithColumn("ProviderSegmentID").AsString(50).Nullable()
                .WithColumn("FlightNo").AsString(50).Nullable()
                .WithColumn("FromAirport").AsString(50).Nullable()
                .WithColumn("ToAirport").AsString(50).Nullable()
                .WithColumn("CabinClass").AsString(50).Nullable()
                .WithColumn("SegmentStatus").AsString(50).Nullable()
                .WithColumn("SegmentFare").AsDecimal().Nullable()
                .WithColumn("CreatedAt").AsDateTime().NotNullable()
                .WithColumn("UpdatedAt").AsDateTime().NotNullable();

            Create.ForeignKey("FK_Segment_Booking")
                .FromTable("Segment").ForeignColumn("BookingID")
                .ToTable("Booking").PrimaryColumn("BookingID");

            Create.Table("Ticket")
                .WithColumn("TicketID").AsInt64().PrimaryKey().Identity()
                .WithColumn("BookingID").AsInt64().NotNullable()
                .WithColumn("TicketNumber").AsString(50).NotNullable()
                .WithColumn("PaxIndex").AsInt32().Nullable()
                .WithColumn("IssueDate").AsDateTime().Nullable()
                .WithColumn("TicketStatus").AsString(50).Nullable()
                .WithColumn("CreatedAt").AsDateTime().NotNullable()
                .WithColumn("UpdatedAt").AsDateTime().NotNullable();

            Create.ForeignKey("FK_Ticket_Booking")
                .FromTable("Ticket").ForeignColumn("BookingID")
                .ToTable("Booking").PrimaryColumn("BookingID");

            Create.Table("HoldTracking")
                .WithColumn("HoldID").AsInt64().PrimaryKey().Identity()
                .WithColumn("BookingID").AsInt64().NotNullable()
                .WithColumn("HoldPlacedAt").AsDateTime().Nullable()
                .WithColumn("ConfirmedAt").AsDateTime().Nullable()
                .WithColumn("Converted").AsBoolean().Nullable()
                .WithColumn("CreatedAt").AsDateTime().NotNullable()
                .WithColumn("UpdatedAt").AsDateTime().NotNullable();

            Create.ForeignKey("FK_HoldTracking_Booking")
                .FromTable("HoldTracking").ForeignColumn("BookingID")
                .ToTable("Booking").PrimaryColumn("BookingID");

            Create.Table("AiRecommendation")
                .WithColumn("RecommendationID").AsInt64().PrimaryKey().Identity()
                .WithColumn("BookingID").AsInt64().Nullable()
                .WithColumn("ActionType").AsString(50).Nullable()
                .WithColumn("Confidence").AsDecimal().Nullable()
                .WithColumn("Status").AsString(50).Nullable()
                .WithColumn("Title").AsString(255).Nullable()
                .WithColumn("Impact").AsString(255).Nullable()
                .WithColumn("SuggestedAction").AsString(255).Nullable()
                .WithColumn("PotentialGain").AsDecimal().Nullable()
                .WithColumn("Context").AsString(int.MaxValue).Nullable()
                .WithColumn("CreatedAt").AsDateTime().NotNullable();

            Create.ForeignKey("FK_AiRecommendation_Booking")
                .FromTable("AiRecommendation").ForeignColumn("BookingID")
                .ToTable("Booking").PrimaryColumn("BookingID");
        }

        public override void Down()
        {
            Delete.ForeignKey("FK_AiRecommendation_Booking").OnTable("AiRecommendation");
            Delete.ForeignKey("FK_HoldTracking_Booking").OnTable("HoldTracking");
            Delete.ForeignKey("FK_Ticket_Booking").OnTable("Ticket");
            Delete.ForeignKey("FK_Segment_Booking").OnTable("Segment");
            Delete.ForeignKey("FK_Ancillary_Booking").OnTable("Ancillary");
            Delete.ForeignKey("FK_Booking_Schedule").OnTable("Booking");
            Delete.ForeignKey("FK_Booking_Provider").OnTable("Booking");
            Delete.ForeignKey("FK_Booking_Agent").OnTable("Booking");
            Delete.ForeignKey("FK_Agent_Provider").OnTable("Agent");

            Delete.Table("AiRecommendation");
            Delete.Table("HoldTracking");
            Delete.Table("Ticket");
            Delete.Table("Segment");
            Delete.Table("Ancillary");
            Delete.Table("Booking");
            Delete.Table("Schedule");
            Delete.Table("Agent");
            Delete.Table("Provider");
        }
    }
}
