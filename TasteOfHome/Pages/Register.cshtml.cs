using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TasteOfHome.Data;

namespace TasteOfHome.Pages
{
    public class RegisterModel : PageModel
    {
        [BindProperty] public string Email { get; set; } = "";
        [BindProperty] public string Password { get; set; } = "";
        public string Message { get; set; } = "";

        public IActionResult OnPost()
        {
            FakeUsers.Users.Add((Email, Password));
            Message = "Account created (demo only)";
            return Page();
        }
    }
}
