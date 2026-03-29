using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TasteOfHome.Data;
using TasteOfHome.Models;

namespace TasteOfHome.Pages.Reservations
{
    [Authorize]
    public class MyReservationsModel : PageModel
    {
        private readonly AppDbContext _db;

        public MyReservationsModel(AppDbContext db)
        {
            _db = db;
        }

        public List<Reservation> Reservations { get; set; } = new();

        public async Task OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

            Reservations = await _db.Reservations
                .AsNoTracking()
                .Include(r => r.Restaurant)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostCancelAsync(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

            var reservation = await _db.Reservations
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (reservation == null)
                return RedirectToPage();

            reservation.Status = "Cancelled";
            await _db.SaveChangesAsync();

            TempData["StatusMessage"] = "Reservation cancelled successfully.";
            return RedirectToPage();
        }
    }
}