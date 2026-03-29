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

        public IndexModel(
            AppDbContext db,
            ISmsSender smsSender)
        {
            _db = db;
            _smsSender = smsSender;
        }

        [BindProperty]
        public List<int> SelectedRestaurantIds { get; set; } = new();

        public List<FeedbackViewModel> FeedbackList { get; set; } = new();
        public List<HiddenGemViewModel> HiddenGemList { get; set; } = new();
        public List<RestaurantViewModel> RestaurantList { get; set; } = new();

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

        public class RestaurantViewModel
        {
            public int Id { get; set; }
            public string Name { get; set; } = "";
            public string Cuisine { get; set; } = "";
            public string Location { get; set; } = "";
            public float Rating { get; set; }
            public int Authenticity { get; set; }
            public int NumberOfReviews { get; set; }
            public string Source { get; set; } = "";
        }

        private bool IsAdminUser()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            return User.Identity?.IsAuthenticated == true &&
                   string.Equals(email, "admin@toh.com", StringComparison.OrdinalIgnoreCase);
        }

        private IActionResult? RedirectIfNotAdmin()
        {
            if (!IsAdminUser())
                return RedirectToPage("/Index");

            return null;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var redirect = RedirectIfNotAdmin();
            if (redirect != null)
                return redirect;

            await LoadFeedbackAsync();
            await LoadHiddenGemsAsync();
            await LoadRestaurantsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteRestaurantAsync(int id)
        {
            var redirect = RedirectIfNotAdmin();
            if (redirect != null)
                return redirect;

            var restaurant = await _db.Restaurants.FirstOrDefaultAsync(r => r.Id == id);
            if (restaurant == null)
            {
                TempData["StatusMessage"] = "Restaurant not found.";
                return RedirectToPage();
            }

            var relatedFeedback = await _db.Feedback
                .Where(f => f.RestaurantId == id)
                .ToListAsync();

            if (relatedFeedback.Any())
            {
                _db.Feedback.RemoveRange(relatedFeedback);
            }

            _db.Restaurants.Remove(restaurant);
            await _db.SaveChangesAsync();

            TempData["StatusMessage"] = $"Restaurant '{restaurant.Name}' deleted successfully.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteSelectedRestaurantsAsync()
        {
            var redirect = RedirectIfNotAdmin();
            if (redirect != null)
                return redirect;

            if (SelectedRestaurantIds == null || !SelectedRestaurantIds.Any())
            {
                TempData["StatusMessage"] = "Please select at least one restaurant to delete.";
                return RedirectToPage();
            }

            var restaurants = await _db.Restaurants
                .Where(r => SelectedRestaurantIds.Contains(r.Id))
                .ToListAsync();

            if (!restaurants.Any())
            {
                TempData["StatusMessage"] = "No matching restaurants found.";
                return RedirectToPage();
            }

            var restaurantIds = restaurants.Select(r => r.Id).ToList();

            var relatedFeedback = await _db.Feedback
                .Where(f => restaurantIds.Contains(f.RestaurantId))
                .ToListAsync();

            if (relatedFeedback.Any())
            {
                _db.Feedback.RemoveRange(relatedFeedback);
            }

            _db.Restaurants.RemoveRange(restaurants);
            await _db.SaveChangesAsync();

            TempData["StatusMessage"] = $"{restaurants.Count} restaurant(s) deleted successfully.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostApproveAsync(int id)
        {
            var redirect = RedirectIfNotAdmin();
            if (redirect != null)
                return redirect;

            var feedback = await _db.Feedback.FirstOrDefaultAsync(f => f.Id == id);
            if (feedback == null)
            {
                TempData["StatusMessage"] = "Feedback not found.";
                return RedirectToPage();
            }

            feedback.Status = "Approved";
            await _db.SaveChangesAsync();

            await RecalculateRestaurantAsync(feedback.RestaurantId);

            TempData["StatusMessage"] = "Feedback approved successfully.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDenyAsync(int id)
        {
            var redirect = RedirectIfNotAdmin();
            if (redirect != null)
                return redirect;

            var feedback = await _db.Feedback.FirstOrDefaultAsync(f => f.Id == id);
            if (feedback == null)
            {
                TempData["StatusMessage"] = "Feedback not found.";
                return RedirectToPage();
            }

            feedback.Status = "Denied";
            await _db.SaveChangesAsync();

            await RecalculateRestaurantAsync(feedback.RestaurantId);

            TempData["StatusMessage"] = "Feedback denied successfully.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostApproveHiddenGemAsync(int id)
        {
            var redirect = RedirectIfNotAdmin();
            if (redirect != null)
                return redirect;

            var hiddenGem = await _db.HiddenGems.FirstOrDefaultAsync(h => h.Id == id);
            if (hiddenGem == null)
            {
                TempData["StatusMessage"] = "Hidden Gem not found.";
                return RedirectToPage();
            }

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

                TempData["StatusMessage"] = "Hidden Gem approved successfully.";
            }
            catch (Exception ex)
            {
                TempData["StatusMessage"] = "Hidden Gem approved, but SMS failed: " + ex.Message;
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDenyHiddenGemAsync(int id)
        {
            var redirect = RedirectIfNotAdmin();
            if (redirect != null)
                return redirect;

            var hiddenGem = await _db.HiddenGems.FirstOrDefaultAsync(h => h.Id == id);
            if (hiddenGem == null)
            {
                TempData["StatusMessage"] = "Hidden Gem not found.";
                return RedirectToPage();
            }

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

                TempData["StatusMessage"] = "Hidden Gem denied successfully.";
            }
            catch (Exception ex)
            {
                TempData["StatusMessage"] = "Hidden Gem denied, but SMS failed: " + ex.Message;
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteHiddenGemAsync(int id)
        {
            var redirect = RedirectIfNotAdmin();
            if (redirect != null)
                return redirect;

            var hiddenGem = await _db.HiddenGems.FirstOrDefaultAsync(h => h.Id == id);
            if (hiddenGem == null)
            {
                TempData["StatusMessage"] = "Hidden Gem not found.";
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
                    Status = string.IsNullOrWhiteSpace(f.Status) ? "Pending" : f.Status
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
                    Status = string.IsNullOrWhiteSpace(h.Status) ? "Pending" : h.Status,
                    CreatedAt = h.CreatedAt
                })
                .ToListAsync();
        }

        private async Task LoadRestaurantsAsync()
        {
            RestaurantList = await _db.Restaurants
                .OrderByDescending(r => r.Id)
                .Select(r => new RestaurantViewModel
                {
                    Id = r.Id,
                    Name = r.Name,
                    Cuisine = r.Cuisine,
                    Location = r.Location,
                    Rating = r.Rating,
                    Authenticity = r.Authenticity,
                    NumberOfReviews = r.NumberOfReviews,
                    Source = r.Source
                })
                .ToListAsync();
        }

        private async Task RecalculateRestaurantAsync(int restaurantId)
        {
            var restaurant = await _db.Restaurants.FirstOrDefaultAsync(r => r.Id == restaurantId);
            if (restaurant == null)
                return;

            var approvedFeedback = await _db.Feedback
                .Where(f => f.RestaurantId == restaurantId && f.Status == "Approved")
                .ToListAsync();

            restaurant.NumberOfReviews = approvedFeedback.Count;

            if (approvedFeedback.Any())
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