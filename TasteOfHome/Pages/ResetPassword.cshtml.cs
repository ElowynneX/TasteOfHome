using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TasteOfHome.Data;
using TasteOfHome.Services;

namespace TasteOfHome.Pages;

public class ResetPasswordModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly PasswordResetService _reset;

    public ResetPasswordModel(AppDbContext db, PasswordResetService reset)
    {
        _db = db;
        _reset = reset;
    }

    [BindProperty(SupportsGet = true)] public string Email { get; set; } = "";
    [BindProperty(SupportsGet = true)] public string Token { get; set; } = "";
    [BindProperty] public string NewPassword { get; set; } = "";

    public string Error { get; set; } = "";
    public string Message { get; set; } = "";

    public async Task<IActionResult> OnGetAsync()
    {
        var (ok, _) = await _reset.ValidateTokenAsync(Email, Token);
        if (!ok) Error = "Invalid or expired reset link.";
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrWhiteSpace(NewPassword) || NewPassword.Length < 6)
        {
            Error = "Password must be at least 6 characters.";
            return Page();
        }

        var (ok, userId) = await _reset.ValidateTokenAsync(Email, Token);
        if (!ok)
        {
            Error = "Invalid or expired reset link.";
            return Page();
        }

        var user = await _db.Users.FindAsync(userId);
        if (user == null)
        {
            Error = "Invalid user.";
            return Page();
        }

        user.PasswordHash = PasswordHasher.Hash(NewPassword); // uses your existing PBKDF2 hasher
        await _db.SaveChangesAsync();

        await _reset.MarkUsedAsync(userId, Token);

        Message = "Password updated. You can now log in.";
        Error = "";
        return Page();
    }
}