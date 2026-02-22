using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TasteOfHome.Data;
using TasteOfHome.Services;

namespace TasteOfHome.Pages
{
    public class LoginModel : PageModel
    {
        private readonly AppDbContext _db;

        public LoginModel(AppDbContext db)
        {
            _db = db;
        }

        [BindProperty] public string Email { get; set; } = "";
        [BindProperty] public string Password { get; set; } = "";
        public string Error { get; set; } = "";

        [BindProperty] public bool RememberMe { get; set; } = true;
        [BindProperty(SupportsGet = true)] public string? ReturnUrl { get; set; }

        public void OnGet()
        {
            ReturnUrl ??= "/";
            if (!Uri.IsWellFormedUriString(ReturnUrl, UriKind.Relative))
                ReturnUrl = "/";
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            var target = returnUrl ?? ReturnUrl ?? "/";
            if (!Uri.IsWellFormedUriString(target, UriKind.Relative))
                target = "/";

            var email = (Email ?? "").Trim().ToLowerInvariant();
            var password = Password ?? "";

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email);

            Console.WriteLine($"[LOGIN] email={email}, found={(user != null)}");
            if (user != null)
                Console.WriteLine($"[LOGIN] hashFormatHasDot={user.PasswordHash.Contains('.')}");

            if (user == null || !PasswordHasher.Verify(password, user.PasswordHash))
            {
                Error = "Invalid login";
                ReturnUrl = target;
                return Page();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, string.IsNullOrWhiteSpace(user.Name) ? user.Email : user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("auth_type", "local")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties { IsPersistent = RememberMe });

            return LocalRedirect(target);
        }
    }
}