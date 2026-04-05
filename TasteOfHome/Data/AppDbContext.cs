using Microsoft.EntityFrameworkCore;
using TasteOfHome.Models;

namespace TasteOfHome.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();
        public DbSet<Restaurant> Restaurants => Set<Restaurant>();
        public DbSet<AppUser> Users => Set<AppUser>();
        public DbSet<Feedback> Feedback => Set<Feedback>();
        public DbSet<HiddenGem> HiddenGems => Set<HiddenGem>();
        public DbSet<Reservation> Reservations => Set<Reservation>();
        public DbSet<CulturalEvent> CulturalEvents => Set<CulturalEvent>();
        public DbSet<EventReservation> EventReservations => Set<EventReservation>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Feedback>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Feedback>()
                .Property(f => f.Status)
                .HasDefaultValue("Approved");

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Restaurant)
                .WithMany()
                .HasForeignKey(r => r.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EventReservation>()
                .HasOne(r => r.CulturalEvent)
                .WithMany(e => e.Reservations)
                .HasForeignKey(r => r.CulturalEventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EventReservation>()
                .Property(r => r.AmountPaid)
                .HasPrecision(10, 2);
        }
    }
}