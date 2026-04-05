using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;
using TasteOfHome.Data;
using TasteOfHome.Models;
using TasteOfHome.Services;

namespace TasteOfHome.Pages.Events
{
    [Authorize]
    public class CheckoutModel : PageModel
    {
        private readonly AppDbContext _db;
        private readonly ILogger<CheckoutModel> _logger;

        public CheckoutModel(AppDbContext db, ILogger<CheckoutModel> logger)
        {
            _db = db;
            _logger = logger;
        }

        public EventReservation Reservation { get; set; } = new();
        public CulturalEvent EventItem { get; set; } = new();
        public string PaymentMessage { get; set; } = "";

        public async Task<IActionResult> OnGetAsync(int reservationId, string? session_id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

            Reservation = await _db.EventReservations
                .Include(r => r.CulturalEvent)
                .FirstOrDefaultAsync(r => r.Id == reservationId && r.UserId == userId) ?? new EventReservation();

            if (Reservation.Id == 0)
                return RedirectToPage("/Error");

            EventItem = Reservation.CulturalEvent ?? new CulturalEvent();

            if (!string.IsNullOrWhiteSpace(session_id))
            {
                try
                {
                    var service = new SessionService();
                    var session = await service.GetAsync(session_id);

                    if (session.PaymentStatus == "paid")
                    {
                        PaymentMessage = "Payment received successfully.";

                        if (Reservation.PaymentStatus != "Paid")
                        {
                            Reservation.PaymentStatus = "Paid";
                            Reservation.Status = "Confirmed";
                            Reservation.StripeCheckoutSessionId = session.Id;
                            Reservation.StripePaymentIntentId = session.PaymentIntentId;

                            if (string.IsNullOrWhiteSpace(Reservation.TicketCode))
                            {
                                Reservation.TicketCode = await GenerateUniqueTicketCodeAsync();
                                Reservation.TicketIssuedAt = DateTime.UtcNow;
                            }

                            if (Reservation.CulturalEvent != null)
                            {
                                var newReservedSpots = Reservation.CulturalEvent.ReservedSpots + Reservation.NumberOfSpots;
                                Reservation.CulturalEvent.ReservedSpots = Math.Min(Reservation.CulturalEvent.Capacity, newReservedSpots);
                            }

                            await _db.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        PaymentMessage = "Payment is still processing.";
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to verify Stripe session {SessionId}", session_id);
                    PaymentMessage = "We could not verify the payment yet. Please refresh in a moment.";
                }
            }
            else
            {
                PaymentMessage = Reservation.PaymentStatus == "Paid"
                    ? "Payment already completed."
                    : "Payment details are not available yet.";
            }

            return Page();
        }

        private async Task<string> GenerateUniqueTicketCodeAsync()
        {
            for (int i = 0; i < 10; i++)
            {
                var code = EventTicketCodeGenerator.Generate();
                var exists = await _db.EventReservations.AnyAsync(r => r.TicketCode == code);
                if (!exists)
                    return code;
            }

            return $"{EventTicketCodeGenerator.Generate()}-{DateTime.UtcNow.Ticks}";
        }
    }
}