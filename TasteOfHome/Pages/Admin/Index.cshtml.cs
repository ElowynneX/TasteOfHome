using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TasteOfHome.Data;

namespace TasteOfHome.Pages.Admin
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _db;

        public IndexModel(AppDbContext db)
        {
            _db = db;
        }

        public List<FeedbackViewModel> FeedbackList { get; set; } = new();

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

        private bool IsAdminUser()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            return User.Identity?.IsAuthenticated == true
                && string.Equals(email, "admin@toh.com", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!IsAdminUser())
            {
                return RedirectToPage("/Index");
            }

            await LoadFeedbackAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostApproveAsync(int id)
        {
            if (!IsAdminUser())
            {
                return RedirectToPage("/Index");
            }

            var feedback = await _db.Feedback.FirstOrDefaultAsync(f => f.Id == id);
            if (feedback == null)
            {
                return RedirectToPage();
            }

            feedback.Status = "Approved";
            await _db.SaveChangesAsync();
            await RecalculateRestaurantAsync(feedback.RestaurantId);

            TempData["StatusMessage"] = "Feedback approved.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDenyAsync(int id)
        {
            if (!IsAdminUser())
            {
                return RedirectToPage("/Index");
            }

            var feedback = await _db.Feedback.FirstOrDefaultAsync(f => f.Id == id);
            if (feedback == null)
            {
                return RedirectToPage();
            }

            feedback.Status = "Denied";
            await _db.SaveChangesAsync();
            await RecalculateRestaurantAsync(feedback.RestaurantId);

            TempData["StatusMessage"] = "Feedback denied.";
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

        private async Task RecalculateRestaurantAsync(int restaurantId)
        {
            var restaurant = await _db.Restaurants.FirstOrDefaultAsync(r => r.Id == restaurantId);
            if (restaurant == null)
            {
                return;
            }

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