using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TasteOfHome.Data;
using System.Security.Claims;
using TasteOfHome.Models;
namespace TasteOfHome.Pages.Reviews
{
    public class DeleteModel : PageModel
    {
        private readonly AppDbContext _db;

        public DeleteModel(AppDbContext db)
        {
            _db = db;
        }

        public IActionResult OnPost(int id)
        {
            var review = _db.Feedback.Find(id);
            if (review == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (review.UserId != userId) return Unauthorized();

            int restaurantId = review.RestaurantId;

            _db.Feedback.Remove(review);
            _db.SaveChanges();

            return RedirectToPage("/Restaurants/Details", new { id = restaurantId });
        }
    }
}