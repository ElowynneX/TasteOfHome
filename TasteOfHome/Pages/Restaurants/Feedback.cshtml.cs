using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TasteOfHome.Data;
using TasteOfHome.Models;

namespace TasteOfHome.Pages.Restaurants
{
    [Authorize]
    public class FeedbackModel : PageModel
    {
        private readonly AppDbContext _db;

        public FeedbackModel(AppDbContext db)
        {
            _db = db;
        }

        public Restaurant Restaurant { get; set; } = new Restaurant();

        [BindProperty]
        public Feedback Feedback { get; set; } = new Feedback();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Restaurant = await _db.Restaurants.FirstOrDefaultAsync(r => r.Id == id) ?? new Restaurant();

            if (Restaurant.Id == 0)
            {
                return Redirect("/Error");
            }

            Feedback.RestaurantId = id;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Restaurant = await _db.Restaurants.FirstOrDefaultAsync(r => r.Id == Feedback.RestaurantId) ?? new Restaurant();

            if (Restaurant.Id == 0)
            {
                return Redirect("/Error");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var feedback = new Feedback
            {
                Rating = Feedback.Rating,
                Authenticity = Feedback.Authenticity,
                Review = Feedback.Review,
                RestaurantId = Feedback.RestaurantId,
                Status = "Pending"
            };

            _db.Feedback.Add(feedback);
            await _db.SaveChangesAsync();

            TempData["StatusMessage"] = "Feedback submitted and is pending admin approval.";
            return RedirectToPage("/Restaurants/Details", new { id = Feedback.RestaurantId });
        }
    }
}