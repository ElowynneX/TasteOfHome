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
    public class CheckInModel : PageModel
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _configuration;

        public CheckInModel(AppDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        [BindProperty]
        public string TicketCode { get; set; } = "";

        public EventReservation? Reservation { get; set; }

        public async Task<IActionResult> OnGetAsync(string? ticket = null)
        {
            if (!IsAdminUser())
                return RedirectToPage("/Index");

            if (!string.IsNullOrWhiteSpace(ticket))
            {
                TicketCode = NormalizeTicket(ticket);
                Reservation = await LoadReservationAsync(TicketCode);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostLookupAsync()
        {
            if (!IsAdminUser())
                return RedirectToPage("/Index");

            TicketCode = NormalizeTicket(TicketCode);
            Reservation = await LoadReservationAsync(TicketCode);

            if (Reservation == null)
            {
                TempData["CheckInError"] = "Ticket not found.";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostConfirmCheckInAsync()
        {
            if (!IsAdminUser())
                return RedirectToPage("/Index");

            TicketCode = NormalizeTicket(TicketCode);

            var reservation = await _db.EventReservations
                .Include(r => r.CulturalEvent)
                .FirstOrDefaultAsync(r => r.TicketCode == TicketCode);

            if (reservation == null)
            {
                TempData["CheckInError"] = "Ticket not found.";
                return RedirectToPage();
            }

            if (reservation.Status == "Cancelled")
            {
                TempData["CheckInError"] = "This ticket is cancelled and cannot be checked in.";
                return RedirectToPage(new { ticket = TicketCode });
            }

            if (reservation.PaymentStatus != "Paid")
            {
                TempData["CheckInError"] = "This booking is not fully paid yet.";
                return RedirectToPage(new { ticket = TicketCode });
            }

            if (reservation.IsCheckedIn)
            {
                TempData["CheckInError"] = "This guest is already checked in.";
                return RedirectToPage(new { ticket = TicketCode });
            }

            reservation.IsCheckedIn = true;
            reservation.CheckedInAt = DateTime.UtcNow;
            reservation.CheckedInByEmail = User.FindFirstValue(ClaimTypes.Email) ?? "Admin";
            reservation.Status = "CheckedIn";

            await _db.SaveChangesAsync();

            TempData["CheckInSuccess"] = "Guest checked in successfully.";
            return RedirectToPage(new { ticket = TicketCode });
        }

        private async Task<EventReservation?> LoadReservationAsync(string ticketCode)
        {
            if (string.IsNullOrWhiteSpace(ticketCode))
                return null;

            return await _db.EventReservations
                .AsNoTracking()
                .Include(r => r.CulturalEvent)
                .FirstOrDefaultAsync(r => r.TicketCode == ticketCode);
        }

        private string NormalizeTicket(string raw)
        {
            raw = (raw ?? "").Trim();

            const string prefix = "TOH-TICKET::";
            if (raw.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                raw = raw.Substring(prefix.Length).Trim();
            }

            return raw;
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