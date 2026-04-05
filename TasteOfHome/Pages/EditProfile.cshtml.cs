using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TasteOfHome.Data;
using TasteOfHome.Models;

namespace TasteOfHome.Pages
{
    [Authorize]
    public class EditProfileModel : PageModel
    {
        private readonly AppDbContext _db;

        public EditProfileModel(AppDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required]
            [Display(Name = "Display Name")]
            [MaxLength(100)]
            public string DisplayName { get; set; } = "";

            [Display(Name = "Location")]
            [MaxLength(100)]
            public string Location { get; set; } = "";
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var email = GetCurrentEmail();
            if (string.IsNullOrWhiteSpace(email))
            {
                return RedirectToPage("/Login");
            }

            var profile = await _db.UserProfiles.FirstOrDefaultAsync(p => p.Email == email);

            if (profile != null)
            {
                Input.DisplayName = profile.DisplayName;
                Input.Location = profile.Location ?? "";
            }
            else
            {
                Input.DisplayName = BuildDisplayName(email);
                Input.Location = "";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var email = GetCurrentEmail();
            if (string.IsNullOrWhiteSpace(email))
            {
                return RedirectToPage("/Login");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var profile = await _db.UserProfiles.FirstOrDefaultAsync(p => p.Email == email);

            if (profile == null)
            {
                profile = new UserProfile
                {
                    Email = email,
                    DisplayName = Input.DisplayName.Trim(),
                    Location = string.IsNullOrWhiteSpace(Input.Location) ? null : Input.Location.Trim()
                };

                _db.UserProfiles.Add(profile);
            }
            else
            {
                profile.DisplayName = Input.DisplayName.Trim();
                profile.Location = string.IsNullOrWhiteSpace(Input.Location) ? null : Input.Location.Trim();
            }

            await _db.SaveChangesAsync();

            TempData["ProfileMessage"] = "Profile updated successfully.";
            return RedirectToPage("/Profile");
        }

        private string GetCurrentEmail()
        {
            var email = User.Identity?.Name ?? "";

            if (string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(FakeUsers.LoggedInEmail))
            {
                email = FakeUsers.LoggedInEmail;
            }

            return email;
        }

        private static string BuildDisplayName(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return "User";

            var localPart = email.Split('@')[0];

            if (string.IsNullOrWhiteSpace(localPart))
                return "User";

            var parts = localPart
                .Replace(".", " ")
                .Replace("_", " ")
                .Replace("-", " ")
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => char.ToUpper(p[0]) + p.Substring(1).ToLower());

            var fullName = string.Join(" ", parts);
            return string.IsNullOrWhiteSpace(fullName) ? "User" : fullName;
        }
    }
}