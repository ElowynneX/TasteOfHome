using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TasteOfHome.Data;
using TasteOfHome.Models;

namespace TasteOfHome.Pages
{
    [Authorize]
    public class SettingsModel : PageModel
    {
        private readonly AppDbContext _db;

        public SettingsModel(AppDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string Email { get; set; } = "";

        public class InputModel
        {
            [Required]
            [Display(Name = "Full Name")]
            [MaxLength(100)]
            public string FullName { get; set; } = "";

            [Display(Name = "Phone Number")]
            [MaxLength(25)]
            public string? PhoneNumber { get; set; }

            [Display(Name = "Email Notifications")]
            public bool EmailNotificationsEnabled { get; set; } = true;

            [Display(Name = "SMS Notifications")]
            public bool SmsNotificationsEnabled { get; set; } = true;

            [Display(Name = "Event Announcements")]
            public bool EventAnnouncementsEnabled { get; set; } = true;

            [Display(Name = "Default Guest Count")]
            [Range(1, 12)]
            public int DefaultGuestCount { get; set; } = 2;

            [Display(Name = "Dietary Preference")]
            [MaxLength(100)]
            public string? DietaryPreference { get; set; }

            [Display(Name = "Seating Preference")]
            [MaxLength(50)]
            public string? SeatingPreference { get; set; }

            [Display(Name = "Marketing Emails")]
            public bool MarketingEmailsEnabled { get; set; } = true;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var email = GetCurrentEmail();
            if (string.IsNullOrWhiteSpace(email))
                return RedirectToPage("/Login");

            Email = email;
            await LoadAsync(email);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var email = GetCurrentEmail();
            if (string.IsNullOrWhiteSpace(email))
                return RedirectToPage("/Login");

            Email = email;

            if (!ModelState.IsValid)
                return Page();

            var settings = await _db.UserSettings.FirstOrDefaultAsync(s => s.Email == email);

            if (settings == null)
            {
                settings = new UserSettings
                {
                    Email = email
                };

                _db.UserSettings.Add(settings);
            }

            settings.FullName = Input.FullName.Trim();
            settings.PhoneNumber = string.IsNullOrWhiteSpace(Input.PhoneNumber) ? null : Input.PhoneNumber.Trim();
            settings.EmailNotificationsEnabled = Input.EmailNotificationsEnabled;
            settings.SmsNotificationsEnabled = Input.SmsNotificationsEnabled;
            settings.EventAnnouncementsEnabled = Input.EventAnnouncementsEnabled;
            settings.DefaultGuestCount = Input.DefaultGuestCount;
            settings.DietaryPreference = string.IsNullOrWhiteSpace(Input.DietaryPreference) ? null : Input.DietaryPreference.Trim();
            settings.SeatingPreference = string.IsNullOrWhiteSpace(Input.SeatingPreference) ? null : Input.SeatingPreference.Trim();
            settings.MarketingEmailsEnabled = Input.MarketingEmailsEnabled;
            settings.UpdatedAt = DateTime.UtcNow;

            var profile = await _db.UserProfiles.FirstOrDefaultAsync(p => p.Email == email);
            if (profile == null)
            {
                profile = new UserProfile
                {
                    Email = email,
                    DisplayName = settings.FullName
                };

                _db.UserProfiles.Add(profile);
            }
            else
            {
                profile.DisplayName = settings.FullName;
            }

            await _db.SaveChangesAsync();

            TempData["SettingsMessage"] = "Your settings were saved successfully.";
            return RedirectToPage();
        }

        private async Task LoadAsync(string email)
        {
            var settings = await _db.UserSettings.AsNoTracking().FirstOrDefaultAsync(s => s.Email == email);
            var profile = await _db.UserProfiles.AsNoTracking().FirstOrDefaultAsync(p => p.Email == email);

            var fallbackName =
                !string.IsNullOrWhiteSpace(profile?.DisplayName) ? profile!.DisplayName :
                !string.IsNullOrWhiteSpace(User.FindFirstValue(ClaimTypes.Name)) ? User.FindFirstValue(ClaimTypes.Name)! :
                BuildDisplayName(email);

            Input = new InputModel
            {
                FullName = settings?.FullName ?? fallbackName,
                PhoneNumber = settings?.PhoneNumber ?? "",
                EmailNotificationsEnabled = settings?.EmailNotificationsEnabled ?? true,
                SmsNotificationsEnabled = settings?.SmsNotificationsEnabled ?? true,
                EventAnnouncementsEnabled = settings?.EventAnnouncementsEnabled ?? true,
                DefaultGuestCount = settings?.DefaultGuestCount ?? 2,
                DietaryPreference = settings?.DietaryPreference ?? "",
                SeatingPreference = settings?.SeatingPreference ?? "",
                MarketingEmailsEnabled = settings?.MarketingEmailsEnabled ?? true
            };
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

        private static string BuildDisplayName(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return "TasteOfHome User";

            var localPart = email.Split('@')[0];

            if (string.IsNullOrWhiteSpace(localPart))
                return "TasteOfHome User";

            var parts = localPart
                .Replace(".", " ")
                .Replace("_", " ")
                .Replace("-", " ")
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => char.ToUpper(p[0]) + p.Substring(1).ToLower());

            var result = string.Join(" ", parts);
            return string.IsNullOrWhiteSpace(result) ? "TasteOfHome User" : result;
        }
    }
}