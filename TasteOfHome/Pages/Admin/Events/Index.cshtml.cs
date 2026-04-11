using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IConfiguration _configuration;

        public IndexModel(AppDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        public List<CulturalEvent> Events { get; set; } = new();

        public int TotalEvents { get; set; }
        public int ActiveEvents { get; set; }
        public int SoldOutEvents { get; set; }
        public int TotalBookings { get; set; }
        public decimal TotalPotentialRevenue { get; set; }
        public int CheckedInGuests { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!IsAdminUser())
                return RedirectToPage("/Index");

            Events = await _db.CulturalEvents
                .OrderByDescending(e => e.CreatedAt)
                .ThenBy(e => e.EventDate)
                .ToListAsync();

            TotalEvents = Events.Count;
            ActiveEvents = Events.Count(e => e.IsActive);
            SoldOutEvents = Events.Count(e => e.Capacity > 0 && e.ReservedSpots >= e.Capacity);
            TotalBookings = Events.Sum(e => e.ReservedSpots);
            TotalPotentialRevenue = Events.Sum(e => e.PricePerPerson * e.ReservedSpots);

            CheckedInGuests = await _db.EventReservations
                .Where(r => r.IsCheckedIn)
                .SumAsync(r => r.NumberOfSpots);

            return Page();
        }

        // ✅ NEW — paste this method here
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            if (!IsAdminUser())
                return RedirectToPage("/Index");

            var ev = await _db.CulturalEvents.FindAsync(id);
            if (ev != null)
            {
                var reservations = _db.EventReservations.Where(r => r.CulturalEventId == id);
                _db.EventReservations.RemoveRange(reservations);

                _db.CulturalEvents.Remove(ev);
                await _db.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Event '{ev.Title}' was deleted successfully.";
            }

            return RedirectToPage();
        }

        private bool IsAdminUser()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            var adminEmails = _configuration
                .GetSection("AdminSettings:AdminEmails")
                .Get<string[]>() ?? Array.Empty<string>();

            return User.Identity?.IsAuthenticated == true
                   && !string.IsNullOrWhiteSpace(userEmail)
                   && adminEmails.Any(a => string.Equals(a, userEmail, StringComparison.OrdinalIgnoreCase));
        }
    }
}