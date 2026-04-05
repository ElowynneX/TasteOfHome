using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TasteOfHome.Data;
using TasteOfHome.Models;

namespace TasteOfHome.Pages.Admin.Events
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _db;

        public IndexModel(AppDbContext db)
        {
            _db = db;
        }

        public List<CulturalEvent> Events { get; set; } = new();

        public int TotalEvents { get; set; }
        public int ActiveEvents { get; set; }
        public int SoldOutEvents { get; set; }
        public int TotalBookings { get; set; }
        public decimal TotalPotentialRevenue { get; set; }

        public async Task OnGetAsync()
        {
            Events = await _db.CulturalEvents
                .OrderByDescending(e => e.CreatedAt)
                .ThenBy(e => e.EventDate)
                .ToListAsync();

            TotalEvents = Events.Count;
            ActiveEvents = Events.Count(e => e.IsActive);
            SoldOutEvents = Events.Count(e => e.Capacity > 0 && e.ReservedSpots >= e.Capacity);
            TotalBookings = Events.Sum(e => e.ReservedSpots);
            TotalPotentialRevenue = Events.Sum(e => e.PricePerPerson * e.ReservedSpots);
        }
    }
}