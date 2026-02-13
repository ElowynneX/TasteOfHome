using Microsoft.EntityFrameworkCore;
using TasteOfHome.Models;

namespace TasteOfHome.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Restaurant> Restaurants => Set<Restaurant>();
        public DbSet<AppUser> Users => Set<AppUser>();
    }
}
