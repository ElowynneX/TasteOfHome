using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TasteOfHome.Data;
using TasteOfHome.Models;

namespace TasteOfHome.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RestaurantsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public RestaurantsController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<List<Restaurant>> GetAll()
        {
            return await _db.Restaurants.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRestaurantById(int id)
        {
            var restaurant = await _db.Restaurants.FirstOrDefaultAsync(r => r.Id == id);

            if (restaurant == null)
            {
                return NotFound();
            }
            return Ok(restaurant);
        }

        //--------THIS ENDPOINT MIGHT GET MOVED TO ANOTHER CONTROLLER--------//
        [HttpPost]
        public async Task<IActionResult> CreateFeedback([FromBody] FeedbackDTO dto)
        {
            var restaurant = await _db.Restaurants.FirstOrDefaultAsync(r => r.Id == dto.RestaurantId);

            if (restaurant == null)
            {
                return NotFound();
            }

            //Create new feedback
            Feedback feedback = new Feedback
            {
                Rating = dto.Rating,
                Authenticity = dto.Authenticity,
                Review = dto.Review,
                RestaurantId = dto.RestaurantId
            };

            _db.Feedback.Add(feedback);
            Console.WriteLine(feedback.ToString());
            await _db.SaveChangesAsync();

            //Update restaurant rating & authenticity
            var feedbackList = await _db.Feedback.Where(f => f.RestaurantId.Equals(dto.RestaurantId)).ToListAsync();
            float totalRating = 0;
            int totalAuthenticity = 0; 

            foreach(var f in feedbackList)
            {
                totalRating += f.Rating;
                totalAuthenticity += f.Authenticity;
            }

            restaurant.NumberOfReviews = feedbackList.Count;
            restaurant.Rating = MathF.Round(totalRating/restaurant.NumberOfReviews, 1);
            restaurant.Authenticity = totalAuthenticity/restaurant.NumberOfReviews;

            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
