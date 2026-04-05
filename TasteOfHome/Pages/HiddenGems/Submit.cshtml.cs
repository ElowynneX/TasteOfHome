using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
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

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            HiddenGem.Status = "Pending";
            HiddenGem.CreatedAt = DateTime.UtcNow;
            HiddenGem.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            HiddenGem.SubmittedByEmail = User.FindFirstValue(ClaimTypes.Email);

            _db.HiddenGems.Add(HiddenGem);
            await _db.SaveChangesAsync();

            _logger.LogInformation(
                "Hidden Gem submitted. Id={Id}, Provider={Provider}, UserId={UserId}, Email={Email}, Status={Status}",
                HiddenGem.Id,
                HiddenGem.ProviderName,
                HiddenGem.UserId,
                HiddenGem.SubmittedByEmail,
                HiddenGem.Status);

            TempData["StatusMessage"] = "Hidden Gem submitted and is pending admin review.";
            return RedirectToPage("/HiddenGems/MySubmissions");
        }
    }
}