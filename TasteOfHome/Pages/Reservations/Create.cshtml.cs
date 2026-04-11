using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TasteOfHome.Data;
using TasteOfHome.Models;
using TasteOfHome.Services;

namespace TasteOfHome.Pages.Reservations
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _db;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(
            AppDbContext db,
            IEmailSender emailSender,
            ISmsSender smsSender,
            ILogger<CreateModel> logger)
        {
            _db = db;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _logger = logger;
        }

        public Restaurant Restaurant { get; set; } = new Restaurant();

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public int MaxGuestsAllowed { get; set; } = 12;
        public bool ReservationsEnabled { get; set; } = true;

        public List<string> TimeSlots { get; } = new()
        {
            "5:00 PM",
            "5:30 PM",
            "6:00 PM",
            "6:30 PM",
            "7:00 PM",
            "7:30 PM",
            "8:00 PM",
            "8:30 PM",
            "9:00 PM"
        };

        public string RestaurantImageUrl =>
            string.IsNullOrWhiteSpace(Restaurant.ImageUrl)
                ? $"/images/restaurants/{GetImageFileName(Restaurant.Id)}"
                : Restaurant.ImageUrl!;

        public class InputModel
        {
            [Required]
            public int RestaurantId { get; set; }

            [Required]
            [StringLength(100)]
            public string CustomerName { get; set; } = "";

            [Required]
            [StringLength(25)]
            public string PhoneNumber { get; set; } = "";

            [Required]
            [DataType(DataType.Date)]
            public DateTime ReservationDate { get; set; } = DateTime.Today.AddDays(1);

            [Required]
            public string ReservationTime { get; set; } = "7:00 PM";

            [Range(1, 12)]
            public int NumberOfGuests { get; set; } = 2;

            [StringLength(500)]
            public string? SpecialRequest { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int restaurantId)
        {
            Restaurant = await _db.Restaurants
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == restaurantId) ?? new Restaurant();

            if (Restaurant.Id == 0)
                return RedirectToPage("/Error");

            var adminSettings = await GetAdminSettingsAsync();
            ReservationsEnabled = adminSettings.EnableRestaurantReservations;
            MaxGuestsAllowed = adminSettings.MaxGuestsPerReservation;

            var userEmail = GetCurrentEmail();
            var userSettings = await GetUserSettingsAsync(userEmail);

            Input.RestaurantId = restaurantId;
            Input.ReservationDate = DateTime.Today.AddDays(1);
            Input.ReservationTime = "7:00 PM";
            Input.NumberOfGuests = Math.Clamp(userSettings?.DefaultGuestCount ?? 2, 1, MaxGuestsAllowed);
            Input.CustomerName = !string.IsNullOrWhiteSpace(userSettings?.FullName)
                ? userSettings!.FullName
                : (User.FindFirstValue(ClaimTypes.Name) ?? "");
            Input.PhoneNumber = userSettings?.PhoneNumber ?? "";
            Input.SpecialRequest = BuildSuggestedReservationNote(userSettings);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Restaurant = await _db.Restaurants
                .FirstOrDefaultAsync(r => r.Id == Input.RestaurantId) ?? new Restaurant();

            if (Restaurant.Id == 0)
                return RedirectToPage("/Error");

            var adminSettings = await GetAdminSettingsAsync();
            ReservationsEnabled = adminSettings.EnableRestaurantReservations;
            MaxGuestsAllowed = adminSettings.MaxGuestsPerReservation;

            if (!ReservationsEnabled)
            {
                ModelState.AddModelError(string.Empty, "Restaurant reservations are currently paused by admin.");
            }

            if (Input.ReservationDate.Date < DateTime.Today)
            {
                ModelState.AddModelError("Input.ReservationDate", "Reservation date cannot be in the past.");
            }

            if (!TimeSlots.Contains(Input.ReservationTime))
            {
                ModelState.AddModelError("Input.ReservationTime", "Please select a valid time slot.");
            }

            if (Input.NumberOfGuests > MaxGuestsAllowed)
            {
                ModelState.AddModelError("Input.NumberOfGuests", $"You can reserve up to {MaxGuestsAllowed} guest(s) right now.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            var userEmail = GetCurrentEmail();
            var userSettings = await GetUserSettingsAsync(userEmail);

            var reservation = new Reservation
            {
                RestaurantId = Input.RestaurantId,
                UserId = userId,
                CustomerName = Input.CustomerName.Trim(),
                PhoneNumber = Input.PhoneNumber.Trim(),
                ReservationDate = Input.ReservationDate.Date,
                ReservationTime = Input.ReservationTime,
                NumberOfGuests = Input.NumberOfGuests,
                SpecialRequest = string.IsNullOrWhiteSpace(Input.SpecialRequest) ? null : Input.SpecialRequest.Trim(),
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            _db.Reservations.Add(reservation);
            await _db.SaveChangesAsync();

            await TrySendRestaurantReservationNotificationsAsync(userEmail, reservation, userSettings);

            TempData["StatusMessage"] =
                $"Reservation request submitted for {Restaurant.Name} on {reservation.ReservationDate:MMMM dd, yyyy} at {reservation.ReservationTime}.";

            return RedirectToPage("/Reservations/MyReservations");
        }

        private async Task TrySendRestaurantReservationNotificationsAsync(string userEmail, Reservation reservation, UserSettings? userSettings)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(userEmail) && (userSettings?.EmailNotificationsEnabled ?? true))
                {
                    var subject = $"TasteOfHome Reservation Received - {Restaurant.Name}";
                    var htmlBody = $@"
                        <div style='font-family:Arial,sans-serif;line-height:1.6;color:#222'>
                            <h2 style='color:#ff6b35;'>Reservation Received</h2>
                            <p>Hi {reservation.CustomerName},</p>
                            <p>Your restaurant reservation request has been received by <strong>TasteOfHome</strong>.</p>

                            <div style='background:#fff4ef;padding:16px;border-radius:12px;border:1px solid #ffd8c7;'>
                                <p><strong>Restaurant:</strong> {Restaurant.Name}</p>
                                <p><strong>Date:</strong> {reservation.ReservationDate:MMMM dd, yyyy}</p>
                                <p><strong>Time:</strong> {reservation.ReservationTime}</p>
                                <p><strong>Guests:</strong> {reservation.NumberOfGuests}</p>
                                <p><strong>Status:</strong> {reservation.Status}</p>
                            </div>

                            <p style='margin-top:16px;'>Thank you for using TasteOfHome.</p>
                        </div>";

                    await _emailSender.SendAsync(userEmail, subject, htmlBody);
                }

                if (!string.IsNullOrWhiteSpace(reservation.PhoneNumber) && (userSettings?.SmsNotificationsEnabled ?? true))
                {
                    var sms =
                        $"TasteOfHome: Hi {reservation.CustomerName}, your reservation request for {Restaurant.Name} on " +
                        $"{reservation.ReservationDate:MMM dd} at {reservation.ReservationTime} for {reservation.NumberOfGuests} guest(s) has been received.";

                    await _smsSender.SendSmsAsync(reservation.PhoneNumber, sms);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send restaurant reservation notification for reservation {ReservationId}", reservation.Id);
                TempData["NotificationWarning"] = "Reservation saved, but confirmation email/SMS could not be sent.";
            }
        }

        private async Task<AdminSettings> GetAdminSettingsAsync()
        {
            var settings = await _db.AdminSettings.AsNoTracking().FirstOrDefaultAsync(s => s.Id == 1);
            return settings ?? new AdminSettings();
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

        private static string? BuildSuggestedReservationNote(UserSettings? userSettings)
        {
            if (userSettings == null)
                return null;

            var parts = new List<string>();

            if (!string.IsNullOrWhiteSpace(userSettings.DietaryPreference))
                parts.Add($"Dietary preference: {userSettings.DietaryPreference}");

            if (!string.IsNullOrWhiteSpace(userSettings.SeatingPreference))
                parts.Add($"Seating preference: {userSettings.SeatingPreference}");

            return parts.Count == 0 ? null : string.Join(" | ", parts);
        }

        public string GetImageFileName(int id)
        {
            return id switch
            {
                1 => "spice-garden.jpg",
                2 => "green-bowl.jpg",
                3 => "golden-wok.jpg",
                4 => "istanbul-grill.jpg",
                5 => "nonna-kitchen.jpg",
                6 => "seoul-street.jpg",
                7 => "pho-saigon.jpg",
                8 => "tokyo-bento.jpg",
                9 => "el-mariachi.jpg",
                10 => "falafel-house.jpg",
                11 => "taste-of-punjab.jpg",
                12 => "bangkok-express.jpg",
                13 => "habesha-table.jpg",
                14 => "casa-latina.jpg",
                15 => "mediterraneo.jpg",
                16 => "karachi-bbq.jpg",
                17 => "plant-power.jpg",
                18 => "la-creperie.jpg",
                19 => "caribbean-flavors.jpg",
                20 => "mama-africa.jpg",
                _ => "default.svg"
            };
        }
    }
}