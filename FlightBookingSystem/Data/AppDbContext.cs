using FlightBookingSystem.Entities;
using FlightBookingSystem.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FlightBookingSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Agent> Agents { get; set; }
        public DbSet<Provider> Providers { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Segment> Segments { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Ancillary> Ancillaries { get; set; }
        public DbSet<HoldTracking> HoldTrackings { get; set; }
        public DbSet<AiRecommendation> AiRecommendations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Agent>().ToTable("Agent");
            modelBuilder.Entity<Provider>().ToTable("Provider");
            modelBuilder.Entity<Booking>().ToTable("Booking");
            modelBuilder.Entity<Schedule>().ToTable("Schedule");
            modelBuilder.Entity<Segment>().ToTable("Segment");
            modelBuilder.Entity<Ticket>().ToTable("Ticket");
            modelBuilder.Entity<Ancillary>().ToTable("Ancillary");
            modelBuilder.Entity<HoldTracking>().ToTable("HoldTracking");
            modelBuilder.Entity<AiRecommendation>().ToTable("AiRecommendation");

            // Booking ↔ Agent

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Agent)
                .WithMany(a => a.Bookings)
                .HasForeignKey(b => b.AgentID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Provider)
                .WithMany(p => p.Bookings)
                .HasForeignKey(b => b.ProviderID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Schedule)
                .WithMany(s => s.Bookings)
                .HasForeignKey(b => b.ScheduleID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Segment>()
                .HasOne(s => s.Booking)
                .WithMany(b => b.Segments)
                .HasForeignKey(s => s.BookingID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Booking)
                .WithMany(b => b.Tickets)
                .HasForeignKey(t => t.BookingID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Ancillary>()
                .HasOne(a => a.Booking)
                .WithMany(b => b.Ancillaries)
                .HasForeignKey(a => a.BookingID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<HoldTracking>()
                .HasOne(h => h.Booking)
                .WithMany(b => b.HoldTrackings)
                .HasForeignKey(h => h.BookingID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AiRecommendation>()
                .HasOne(r => r.Booking)
                .WithMany(b => b.AiRecommendations)
                .HasForeignKey(r => r.BookingID)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Booking>()
           .Property(b => b.Status)
           .HasConversion(new EnumToStringConverter<BookingStatus>());
        }
    }
}
