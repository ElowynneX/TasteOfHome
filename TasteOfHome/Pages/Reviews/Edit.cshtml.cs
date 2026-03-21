using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TasteOfHome.Data;
using TasteOfHome.Models;
using System.Security.Claims;

namespace TasteOfHome.Pages.Reviews
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _db;

        public EditModel(AppDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public Feedback Review { get; set; }

        public IActionResult OnGet(int id)
        {
            var review = _db.Feedback.Find(id);
            if (review == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (review.UserId != userId) return Unauthorized();

            Review = review;
            return Page();
        }

        public IActionResult OnPost()
        {
            var review = _db.Feedback.Find(Review.Id);
            if (review == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (review.UserId != userId) return Unauthorized();

            review.Rating = Review.Rating;
            review.Review = Review.Review;
            review.Authenticity = Review.Authenticity;

            _db.SaveChanges();

            return RedirectToPage("/Restaurants/Details", new { id = review.RestaurantId });
        }
    }
}