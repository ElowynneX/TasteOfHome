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
            var feedbackList = await _db.Feedback.ToListAsync();
            int totalRating = 0;
            int totalAuthenticity = 0; 
            int relevantRestaurantCounter = 0;

            foreach(var f in feedbackList)
            {
                if (f.RestaurantId == restaurant.Id)
                {
                    relevantRestaurantCounter += 1;
                    totalRating += f.Rating;
                    totalAuthenticity += f.Authenticity;
                }
            }

            restaurant.Rating = totalRating/relevantRestaurantCounter;
            restaurant.Authenticity = totalAuthenticity/relevantRestaurantCounter;

            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
