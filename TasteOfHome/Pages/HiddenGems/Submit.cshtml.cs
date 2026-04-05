using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TasteOfHome.Data;
using TasteOfHome.Models;

namespace TasteOfHome.Pages.HiddenGems
{
    [Authorize]
    public class SubmitModel : PageModel
    {
        private readonly AppDbContext _db;
        private readonly ILogger<SubmitModel> _logger;

        public SubmitModel(AppDbContext db, ILogger<SubmitModel> logger)
        {
            _db = db;
            _logger = logger;
        }

        [BindProperty]
        public HiddenGem HiddenGem { get; set; } = new HiddenGem();

        public bool HiddenGemSubmissionsEnabled { get; set; } = true;
        public bool RequireApprovalWorkflow { get; set; } = true;

        public async Task OnGetAsync()
        {
            var adminSettings = await GetAdminSettingsAsync();
            HiddenGemSubmissionsEnabled = adminSettings.EnableHiddenGemSubmissions;
            RequireApprovalWorkflow = adminSettings.RequireHiddenGemApproval;

            var userSettings = await GetUserSettingsAsync(GetCurrentEmail());
            if (!string.IsNullOrWhiteSpace(userSettings?.PhoneNumber))
            {
                HiddenGem.SubmitterPhoneNumber = userSettings.PhoneNumber!;
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var adminSettings = await GetAdminSettingsAsync();
            HiddenGemSubmissionsEnabled = adminSettings.EnableHiddenGemSubmissions;
            RequireApprovalWorkflow = adminSettings.RequireHiddenGemApproval;

            if (!HiddenGemSubmissionsEnabled)
            {
                ModelState.AddModelError(string.Empty, "Hidden gem submissions are currently paused by admin.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            HiddenGem.Status = RequireApprovalWorkflow ? "Pending" : "Approved";
            HiddenGem.CreatedAt = DateTime.UtcNow;
            HiddenGem.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            HiddenGem.SubmittedByEmail = GetCurrentEmail();

            if (!RequireApprovalWorkflow)
            {
                HiddenGem.ReviewedAt = DateTime.UtcNow;
                HiddenGem.ReviewedByEmail = "System";
            }

            _db.HiddenGems.Add(HiddenGem);
            await _db.SaveChangesAsync();

            _logger.LogInformation(
                "Hidden Gem submitted. Id={Id}, Provider={Provider}, UserId={UserId}, Email={Email}, Status={Status}",
                HiddenGem.Id,
                HiddenGem.ProviderName,
                HiddenGem.UserId,
                HiddenGem.SubmittedByEmail,
                HiddenGem.Status);

            TempData["StatusMessage"] = RequireApprovalWorkflow
                ? "Hidden Gem submitted and is pending admin review."
                : "Hidden Gem submitted and published successfully.";

            return RedirectToPage("/HiddenGems/MySubmissions");
        }

        private async Task<TasteOfHome.Models.AdminSettings> GetAdminSettingsAsync()
        {
            var settings = await _db.AdminSettings.AsNoTracking().FirstOrDefaultAsync(s => s.Id == 1);
            return settings ?? new TasteOfHome.Models.AdminSettings();
        }

        private async Task<UserSettings?> GetUserSettingsAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            return await _db.UserSettings.AsNoTracking().FirstOrDefaultAsync(s => s.Email == email);
        }

        private string GetCurrentEmail()
        {
            var email = User.FindFirstValue(ClaimTypes.Email) ?? "";

            if (string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(FakeUsers.LoggedInEmail))
            {
                email = FakeUsers.LoggedInEmail;
            }

            return email.Trim();
        }
    }
}