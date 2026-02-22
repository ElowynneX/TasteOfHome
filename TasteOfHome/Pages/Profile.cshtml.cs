using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Authorize]
public class ProfileModel : PageModel
{
    public void OnGet()
    {
    }
}