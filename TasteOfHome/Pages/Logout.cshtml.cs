using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TasteOfHome.Data;
using Microsoft.AspNetCore.Authentication;

namespace TasteOfHome.Pages
{
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnPostAsync()
        {
            FakeUsers.LoggedInEmail = null;

            await HttpContext.SignOutAsync("Cookies");
            await HttpContext.SignOutAsync("Google");

            return RedirectToPage("/Index");
        }
    }
}
