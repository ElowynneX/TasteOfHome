using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TasteOfHome.Data;
using TasteOfHome.Services;

namespace TasteOfHome.Pages.Admin
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _db;
        private readonly ISmsSender _smsSender;

        public IndexModel(AppDbContext db, ISmsSender smsSender)
        {
            _db = db;
            _smsSender = smsSender;
        }

        public List<FeedbackViewModel> FeedbackList { get; set; } = new();
        public List<HiddenGemViewModel> HiddenGemList { get; set; } = new();

        public class FeedbackViewModel
        {
            public int Id { get; set; }
            public string RestaurantName { get; set; } = "";
            public int RestaurantId { get; set; }
            public int Rating { get; set; }
            public int Authenticity { get; set; }
            public string Review { get; set; } = "";
            public string Status { get; set; } = "";
        }

        public class HiddenGemViewModel
        {
            public int Id { get; set; }
            public string ProviderName { get; set; } = "";
            public string Location { get; set; } = "";
            public string FoodType { get; set; } = "";
            public string Description { get; set; } = "";
            public string? ContactInfo { get; set; }
            public string SubmitterPhoneNumber { get; set; } = "";
            public int AuthenticityHint { get; set; }
            public string Status { get; set; } = "";
            public DateTime CreatedAt { get; set; }
        }

        private bool IsAdminUser()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            return User.Identity?.IsAuthenticated == true
                && string.Equals(email, "admin@toh.com", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!IsAdminUser())
                return RedirectToPage("/Index");

            await LoadFeedbackAsync();
            await LoadHiddenGemsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostApproveAsync(int id)
        {
            if (!IsAdminUser())
                return RedirectToPage("/Index");

            var feedback = await _db.Feedback.FirstOrDefaultAsync(f => f.Id == id);
            if (feedback == null)
                return RedirectToPage();

            feedback.Status = "Approved";
            await _db.SaveChangesAsync();
            await RecalculateRestaurantAsync(feedback.RestaurantId);

            TempData["StatusMessage"] = "Feedback approved.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDenyAsync(int id)
        {
            if (!IsAdminUser())
                return RedirectToPage("/Index");

            var feedback = await _db.Feedback.FirstOrDefaultAsync(f => f.Id == id);
            if (feedback == null)
                return RedirectToPage();

            feedback.Status = "Denied";
            await _db.SaveChangesAsync();
            await RecalculateRestaurantAsync(feedback.RestaurantId);

            TempData["StatusMessage"] = "Feedback denied.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostApproveHiddenGemAsync(int id)
        {
            if (!IsAdminUser())
                return RedirectToPage("/Index");

            var hiddenGem = await _db.HiddenGems.FirstOrDefaultAsync(h => h.Id == id);
            if (hiddenGem == null)
                return RedirectToPage();

            hiddenGem.Status = "Approved";
            await _db.SaveChangesAsync();

            try
            {
                if (!string.IsNullOrWhiteSpace(hiddenGem.SubmitterPhoneNumber))
                {
                    await _smsSender.SendSmsAsync(
                        hiddenGem.SubmitterPhoneNumber,
                        "TasteOfHome: Your Hidden Gem submission was approved."
                    );
                }

                TempData["StatusMessage"] = "Hidden Gem approved and SMS sent.";
            }
            catch (Exception ex)
            {
                TempData["StatusMessage"] = "Hidden Gem approved, but SMS failed: " + ex.Message;
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDenyHiddenGemAsync(int id)
        {
            if (!IsAdminUser())
                return RedirectToPage("/Index");

            var hiddenGem = await _db.HiddenGems.FirstOrDefaultAsync(h => h.Id == id);
            if (hiddenGem == null)
                return RedirectToPage();

            hiddenGem.Status = "Denied";
            await _db.SaveChangesAsync();

            try
            {
                if (!string.IsNullOrWhiteSpace(hiddenGem.SubmitterPhoneNumber))
                {
                    await _smsSender.SendSmsAsync(
                        hiddenGem.SubmitterPhoneNumber,
                        "TasteOfHome: Your Hidden Gem submission was denied."
                    );
                }

                TempData["StatusMessage"] = "Hidden Gem denied and SMS sent.";
            }
            catch (Exception ex)
            {
                TempData["StatusMessage"] = "Hidden Gem denied, but SMS failed: " + ex.Message;
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteHiddenGemAsync(int id)
        {
            if (!IsAdminUser())
            {
                return RedirectToPage("/Index");
            }

            var hiddenGem = await _db.HiddenGems.FirstOrDefaultAsync(h => h.Id == id);
            if (hiddenGem == null)
            {
                return RedirectToPage();
            }

            _db.HiddenGems.Remove(hiddenGem);
            await _db.SaveChangesAsync();

            TempData["StatusMessage"] = "Hidden Gem deleted successfully.";
            return RedirectToPage();
        }

        private async Task LoadFeedbackAsync()
        {
            FeedbackList = await (
                from f in _db.Feedback
                join r in _db.Restaurants on f.RestaurantId equals r.Id
                orderby f.Id descending
                select new FeedbackViewModel
                {
                    Id = f.Id,
                    RestaurantId = f.RestaurantId,
                    RestaurantName = r.Name,
                    Rating = f.Rating,
                    Authenticity = f.Authenticity,
                    Review = f.Review,
                    Status = string.IsNullOrWhiteSpace(f.Status) ? "Approved" : f.Status
                }
            ).ToListAsync();
        }

        private async Task LoadHiddenGemsAsync()
        {
            HiddenGemList = await _db.HiddenGems
                .OrderByDescending(h => h.CreatedAt)
                .Select(h => new HiddenGemViewModel
                {
                    Id = h.Id,
                    ProviderName = h.ProviderName,
                    Location = h.Location,
                    FoodType = h.FoodType,
                    Description = h.Description,
                    ContactInfo = h.ContactInfo,
                    SubmitterPhoneNumber = h.SubmitterPhoneNumber,
                    AuthenticityHint = h.AuthenticityHint,
                    Status = h.Status,
                    CreatedAt = h.CreatedAt
                })
                .ToListAsync();
        }

        private async Task RecalculateRestaurantAsync(int restaurantId)
        {
            var restaurant = await _db.Restaurants.FirstOrDefaultAsync(r => r.Id == restaurantId);
            if (restaurant == null)
                return;

            var approvedFeedback = await _db.Feedback
                .Where(f => f.RestaurantId == restaurantId && (f.Status == "Approved" || f.Status == null))
                .ToListAsync();

            restaurant.NumberOfReviews = approvedFeedback.Count;

            if (approvedFeedback.Count > 0)
            {
                restaurant.Rating = (float)Math.Round(approvedFeedback.Average(f => f.Rating), 1);
                restaurant.Authenticity = (int)Math.Round(approvedFeedback.Average(f => f.Authenticity));
            }
            else
            {
                restaurant.Rating = 0;
                restaurant.Authenticity = 0;
            }

            await _db.SaveChangesAsync();
        }
    }
}