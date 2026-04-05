using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TasteOfHome.Data;
using TasteOfHome.Models;

namespace TasteOfHome.Pages.Events
{
    [Authorize]
    public class MyBookingsModel : PageModel
    {
        private readonly AppDbContext _db;

        public MyBookingsModel(AppDbContext db)
        {
            _db = db;
        }

        public List<EventReservation> Bookings { get; set; } = new();
        public string QrPayloadPrefix => "TOH-TICKET::";

        public async Task OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

            Bookings = await _db.EventReservations
                .AsNoTracking()
                .Include(r => r.CulturalEvent)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostCancelAsync(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

            var booking = await _db.EventReservations
                .Include(r => r.CulturalEvent)
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (booking == null)
                return RedirectToPage();

            if (booking.Status != "Cancelled")
            {
                booking.Status = "Cancelled";

                if (booking.CulturalEvent != null)
                {
                    booking.CulturalEvent.ReservedSpots = Math.Max(0, booking.CulturalEvent.ReservedSpots - booking.NumberOfSpots);
                }

                await _db.SaveChangesAsync();
            }

            TempData["StatusMessage"] = "Hosted event booking cancelled successfully.";
            return RedirectToPage();
        }
    }
}