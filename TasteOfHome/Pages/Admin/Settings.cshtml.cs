using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TasteOfHome.Data;

namespace TasteOfHome.Pages.Admin
{
    [Authorize]
    public class SettingsModel : PageModel
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _configuration;

        public SettingsModel(AppDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public DateTime UpdatedAt { get; set; }

        public class InputModel
        {
            [Display(Name = "Enable Restaurant Reservations")]
            public bool EnableRestaurantReservations { get; set; } = true;

            [Display(Name = "Enable Event Bookings")]
            public bool EnableEventBookings { get; set; } = true;

            [Display(Name = "Enable Hidden Gem Submissions")]
            public bool EnableHiddenGemSubmissions { get; set; } = true;

            [Display(Name = "Require Hidden Gem Approval")]
            public bool RequireHiddenGemApproval { get; set; } = true;

            [Display(Name = "Show Hidden Gems On Homepage")]
            public bool ShowHiddenGemsOnHomepage { get; set; } = false;

            [Display(Name = "Max Guests Per Reservation")]
            [Range(1, 12)]
            public int MaxGuestsPerReservation { get; set; } = 12;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!IsAdminUser())
                return RedirectToPage("/Index");

            var settings = await GetOrCreateSettingsAsync();

            Input = new InputModel
            {
                EnableRestaurantReservations = settings.EnableRestaurantReservations,
                EnableEventBookings = settings.EnableEventBookings,
                EnableHiddenGemSubmissions = settings.EnableHiddenGemSubmissions,
                RequireHiddenGemApproval = settings.RequireHiddenGemApproval,
                ShowHiddenGemsOnHomepage = settings.ShowHiddenGemsOnHomepage,
                MaxGuestsPerReservation = settings.MaxGuestsPerReservation
            };

            UpdatedAt = settings.UpdatedAt;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!IsAdminUser())
                return RedirectToPage("/Index");

            if (!ModelState.IsValid)
                return Page();

            var settings = await GetOrCreateSettingsAsync();

            settings.EnableRestaurantReservations = Input.EnableRestaurantReservations;
            settings.EnableEventBookings = Input.EnableEventBookings;
            settings.EnableHiddenGemSubmissions = Input.EnableHiddenGemSubmissions;
            settings.RequireHiddenGemApproval = Input.RequireHiddenGemApproval;
            settings.ShowHiddenGemsOnHomepage = Input.ShowHiddenGemsOnHomepage;
            settings.MaxGuestsPerReservation = Input.MaxGuestsPerReservation;
            settings.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            TempData["AdminSettingsMessage"] = "Admin settings saved successfully.";
            return RedirectToPage();
        }

        private async Task<TasteOfHome.Models.AdminSettings> GetOrCreateSettingsAsync()
        {
            var settings = await _db.AdminSettings.FirstOrDefaultAsync(s => s.Id == 1);

            if (settings != null)
                return settings;

            settings = new TasteOfHome.Models.AdminSettings
            {
                Id = 1,
                UpdatedAt = DateTime.UtcNow
            };

            _db.AdminSettings.Add(settings);
            await _db.SaveChangesAsync();

            return settings;
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