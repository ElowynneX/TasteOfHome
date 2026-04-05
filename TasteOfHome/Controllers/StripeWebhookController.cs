using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using TasteOfHome.Data;
using TasteOfHome.Models;
using TasteOfHome.Services;

namespace TasteOfHome.Controllers
{
    [ApiController]
    [Route("api/stripe/webhook")]
    public class StripeWebhookController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly StripeOptions _stripeOptions;
        private readonly ILogger<StripeWebhookController> _logger;

        public StripeWebhookController(
            AppDbContext db,
            IOptions<StripeOptions> stripeOptions,
            ILogger<StripeWebhookController> logger)
        {
            _db = db;
            _stripeOptions = stripeOptions.Value;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Handle()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            Event stripeEvent;

            try
            {
                stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    _stripeOptions.WebhookSecret
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Stripe webhook signature verification failed.");
                return BadRequest();
            }

            if (stripeEvent.Type == "checkout.session.completed")
            {
                var session = stripeEvent.Data.Object as Session;

                if (session != null &&
                    session.Metadata != null &&
                    session.Metadata.TryGetValue("reservation_id", out var reservationIdRaw) &&
                    int.TryParse(reservationIdRaw, out var reservationId))
                {
                    var reservation = await _db.EventReservations
                        .Include(r => r.CulturalEvent)
                        .FirstOrDefaultAsync(r => r.Id == reservationId);

                    if (reservation != null && reservation.PaymentStatus != "Paid")
                    {
                        reservation.PaymentStatus = "Paid";
                        reservation.Status = "Confirmed";
                        reservation.StripeCheckoutSessionId = session.Id;
                        reservation.StripePaymentIntentId = session.PaymentIntentId;

                        if (string.IsNullOrWhiteSpace(reservation.TicketCode))
                        {
                            reservation.TicketCode = await GenerateUniqueTicketCodeAsync();
                            reservation.TicketIssuedAt = DateTime.UtcNow;
                        }

                        if (reservation.CulturalEvent != null)
                        {
                            var newReservedSpots = reservation.CulturalEvent.ReservedSpots + reservation.NumberOfSpots;
                            reservation.CulturalEvent.ReservedSpots = Math.Min(reservation.CulturalEvent.Capacity, newReservedSpots);
                        }

                        await _db.SaveChangesAsync();
                    }
                }
            }

            return Ok();
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