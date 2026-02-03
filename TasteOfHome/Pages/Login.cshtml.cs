using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TasteOfHome.Data;

namespace TasteOfHome.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty] public string Email { get; set; } = "";
        [BindProperty] public string Password { get; set; } = "";
        public string Error { get; set; } = "";

        public IActionResult OnPost()
        {
            var match = FakeUsers.Users
                .Any(u => u.Email == Email && u.Password == Password);

            if (!match)
            {
                Error = "Invalid login";
                return Page();
            }

            // success
            FakeUsers.LoggedInEmail = Email;
            return RedirectToPage("/Index");
        }
    }
}
