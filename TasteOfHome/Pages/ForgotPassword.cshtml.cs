using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TasteOfHome.Services;

namespace TasteOfHome.Pages;

public class ForgotPasswordModel : PageModel
{
    private readonly PasswordResetService _reset;
    private readonly IEmailSender _email;
    private readonly IConfiguration _cfg;

    public ForgotPasswordModel(PasswordResetService reset, IEmailSender email, IConfiguration cfg)
    {
        _reset = reset;
        _email = email;
        _cfg = cfg;
    }

    [BindProperty] public string Email { get; set; } = "";
    public string Message { get; set; } = "";
    public string Error { get; set; } = "";

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        var email = (Email ?? "").Trim();

        // generic response prevents account enumeration
        Message = "If an account exists for that email, a reset link has been sent. Check your spam folder";
        Error = "";

        var token = await _reset.CreateResetTokenAsync(email);
        if (token == null) return Page();

        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var link = $"{baseUrl}/ResetPassword?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(token)}";

        var body = $@"
<p>Reset your TasteOfHome password:</p>
<p><a href=""{link}"">Reset Password</a></p>
<p>This link expires in 30 minutes.</p>";

        await _email.SendAsync(email, "TasteOfHome Password Reset", body);
        return Page();
    }
}