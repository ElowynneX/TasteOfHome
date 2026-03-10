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
        public DbSet<HiddenGem> HiddenGems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Feedback>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Feedback>()
                .Property(f => f.Status)
                .HasDefaultValue("Approved");
        }
    }
}