using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TasteOfHome.Data;

namespace TasteOfHome.Pages
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnPost()
        {
            FakeUsers.LoggedInEmail = null;
            return RedirectToPage("/Index");
        }
    }
}
