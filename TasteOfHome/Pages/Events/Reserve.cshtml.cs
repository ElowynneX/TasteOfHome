using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe.Checkout;
using TasteOfHome.Data;
using TasteOfHome.Models;

namespace TasteOfHome.Pages.Events
{
    [Authorize]
    public class ReserveModel : PageModel
    {
        private readonly AppDbContext _db;
        private readonly StripeOptions _stripeOptions;
        private readonly ILogger<ReserveModel> _logger;

        public ReserveModel(
            AppDbContext db,
            IOptions<StripeOptions> stripeOptions,
            ILogger<ReserveModel> logger)
        {
            _db = db;
            _stripeOptions = stripeOptions.Value;
            _logger = logger;
        }

        public CulturalEvent EventItem { get; set; } = new CulturalEvent();

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public bool EventBookingsEnabled { get; set; } = true;

        public int RemainingSpots => Math.Max(0, EventItem.Capacity - EventItem.ReservedSpots);
        public int MaxSelectableSpots => Math.Min(6, Math.Max(1, RemainingSpots));

        public bool IsStripeConfigured =>
            !string.IsNullOrWhiteSpace(_stripeOptions.SecretKey) &&
            !string.IsNullOrWhiteSpace(_stripeOptions.PublishableKey) &&
            !string.IsNullOrWhiteSpace(_stripeOptions.Currency);

        public bool CanReserve =>
            EventBookingsEnabled &&
            EventItem.Id > 0 &&
            EventItem.IsActive &&
            EventItem.EventDate.Date >= DateTime.Today &&
            RemainingSpots > 0 &&
            IsStripeConfigured;

        public class InputModel
        {
            [Required]
            public int CulturalEventId { get; set; }

            [Required]
            [StringLength(100)]
            public string CustomerName { get; set; } = "";

            [Required]
            [EmailAddress]
            [StringLength(150)]
            public string Email { get; set; } = "";

            [Required]
            [StringLength(25)]
            public string PhoneNumber { get; set; } = "";

            [Range(1, 6)]
            public int NumberOfSpots { get; set; } = 1;

            [StringLength(500)]
            public string? Notes { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int eventId)
        {
            EventItem = await _db.CulturalEvents
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == eventId) ?? new CulturalEvent();

            if (EventItem.Id == 0)
                return RedirectToPage("/Error");

            var adminSettings = await GetAdminSettingsAsync();
            EventBookingsEnabled = adminSettings.EnableEventBookings;

            var userEmail = GetCurrentEmail();
            var userSettings = await GetUserSettingsAsync(userEmail);

            Input.CulturalEventId = eventId;
            Input.CustomerName = !string.IsNullOrWhiteSpace(userSettings?.FullName)
                ? userSettings!.FullName
                : (User.FindFirstValue(ClaimTypes.Name) ?? "");
            Input.Email = userEmail;
            Input.PhoneNumber = userSettings?.PhoneNumber ?? "";
            Input.NumberOfSpots = 1;
            Input.Notes = BuildSuggestedEventNote(userSettings);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            EventItem = await _db.CulturalEvents
                .FirstOrDefaultAsync(e => e.Id == Input.CulturalEventId) ?? new CulturalEvent();

            if (EventItem.Id == 0)
                return RedirectToPage("/Error");

            var adminSettings = await GetAdminSettingsAsync();
            EventBookingsEnabled = adminSettings.EnableEventBookings;

            if (!EventBookingsEnabled)
                ModelState.AddModelError(string.Empty, "Event bookings are currently paused by admin.");

            if (!IsStripeConfigured)
                ModelState.AddModelError(string.Empty, "Payment is not configured yet. Please add Stripe keys first.");

            if (!EventItem.IsActive || EventItem.EventDate.Date < DateTime.Today)
                ModelState.AddModelError(string.Empty, "This event is no longer available for booking.");

            if (RemainingSpots <= 0)
                ModelState.AddModelError(string.Empty, "This event is sold out.");

            if (Input.NumberOfSpots > RemainingSpots)
                ModelState.AddModelError("Input.NumberOfSpots", $"Only {RemainingSpots} spot(s) left for this event.");

            if (!ModelState.IsValid)
                return Page();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            var amount = EventItem.PricePerPerson * Input.NumberOfSpots;

            var reservation = new EventReservation
            {
                CulturalEventId = Input.CulturalEventId,
                UserId = userId,
                CustomerName = Input.CustomerName.Trim(),
                Email = Input.Email.Trim(),
                PhoneNumber = Input.PhoneNumber.Trim(),
                NumberOfSpots = Input.NumberOfSpots,
                Notes = string.IsNullOrWhiteSpace(Input.Notes) ? null : Input.Notes.Trim(),
                Status = "PendingPayment",
                PaymentStatus = "Pending",
                AmountPaid = amount,
                CreatedAt = DateTime.UtcNow
            };

            _db.EventReservations.Add(reservation);
            await _db.SaveChangesAsync();

            try
            {
                var baseUrl = $"{Request.Scheme}://{Request.Host}";

                var sessionOptions = new SessionCreateOptions
                {
                    Mode = "payment",
                    PaymentMethodTypes = new List<string> { "card" },
                    SuccessUrl = $"{baseUrl}/Events/Checkout?reservationId={reservation.Id}&session_id={{CHECKOUT_SESSION_ID}}",
                    CancelUrl = $"{baseUrl}/Events/Reserve/{EventItem.Id}",
                    CustomerEmail = reservation.Email,
                    ClientReferenceId = reservation.UserId,
                    Metadata = new Dictionary<string, string>
                    {
                        ["reservation_id"] = reservation.Id.ToString(),
                        ["event_id"] = EventItem.Id.ToString(),
                        ["customer_email"] = reservation.Email
                    },
                    LineItems = new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions
                        {
                            Quantity = 1,
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                Currency = _stripeOptions.Currency,
                                UnitAmount = (long)(amount * 100),
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = $"{EventItem.Title} - {reservation.NumberOfSpots} spot(s)",
                                    Description = $"{EventItem.EventDate:MMMM dd, yyyy} • {EventItem.VenueName}, {EventItem.City}"
                                }
                            }
                        }
                    }
                };

                var service = new SessionService();
                var session = await service.CreateAsync(sessionOptions);

                reservation.StripeCheckoutSessionId = session.Id;
                await _db.SaveChangesAsync();

                return Redirect(session.Url);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Stripe session creation failed for reservation {ReservationId}", reservation.Id);

                reservation.Status = "PaymentFailed";
                reservation.PaymentStatus = "Failed";
                await _db.SaveChangesAsync();

                ModelState.AddModelError(string.Empty, "We could not open the payment page. Please try again.");
                return Page();
            }
        }

        private async Task<TasteOfHome.Models.AdminSettings> GetAdminSettingsAsync()
        {
            var settings = await _db.AdminSettings.AsNoTracking().FirstOrDefaultAsync(s => s.Id == 1);
            return settings ?? new TasteOfHome.Models.AdminSettings();
        }

        private async Task<UserSettings?> GetUserSettingsAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            return await _db.UserSettings.AsNoTracking().FirstOrDefaultAsync(s => s.Email == email);
        }

        private string GetCurrentEmail()
        {
            var email = User.FindFirstValue(ClaimTypes.Email) ?? "";

            if (string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(FakeUsers.LoggedInEmail))
            {
                email = FakeUsers.LoggedInEmail;
            }

            return email.Trim();
        }

        private static string? BuildSuggestedEventNote(UserSettings? userSettings)
        {
            if (userSettings == null || string.IsNullOrWhiteSpace(userSettings.DietaryPreference))
                return null;

            return $"Dietary preference: {userSettings.DietaryPreference}";
        }
    }
}