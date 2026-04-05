using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TasteOfHome.Data;

namespace TasteOfHome.Pages
{
    [Authorize]
    public class EditProfileModel : PageModel
    {
        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required]
            [Display(Name = "Display Name")]
            public string DisplayName { get; set; } = "";

            [Display(Name = "Location")]
            public string Location { get; set; } = "";
        }

        public void OnGet()
        {
            var email = User.Identity?.Name ?? "";

            if (string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(FakeUsers.LoggedInEmail))
            {
                email = FakeUsers.LoggedInEmail;
            }

            Input.DisplayName = BuildDisplayName(email);
            Input.Location = "TasteOfHome Member";
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            TempData["ProfileMessage"] = "Profile updated successfully.";
            return RedirectToPage("/Profile");
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