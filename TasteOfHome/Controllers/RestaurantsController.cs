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
    }
}
