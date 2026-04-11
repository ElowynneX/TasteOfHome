using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TasteOfHome.Data;
using TasteOfHome.Models;

namespace TasteOfHome.Pages.Restaurants
{
    public class DetailsModel : PageModel
    {
        private readonly AppDbContext _db;

        public DetailsModel(AppDbContext db)
        {
            _db = db;
        }

        public Restaurant Restaurant { get; set; } = new Restaurant();
        public List<Feedback> RestaurantFeedback { get; set; } = new();

        public string GetImageFileName(int id)
        {
            return id switch
            {
                1 => "spice-garden.jpg",
                2 => "green-bowl.jpg",
                3 => "golden-wok.jpg",
                4 => "istanbul-grill.jpg",
                5 => "nonna-kitchen.jpg",
                6 => "seoul-street.jpg",
                7 => "pho-saigon.jpg",
                8 => "tokyo-bento.jpg",
                9 => "el-mariachi.jpg",
                10 => "falafel-house.jpg",
                11 => "taste-of-punjab.jpg",
                12 => "bangkok-express.jpg",
                13 => "habesha-table.jpg",
                14 => "casa-latina.jpg",
                15 => "mediterraneo.jpg",
                16 => "karachi-bbq.jpg",
                17 => "plant-power.jpg",
                18 => "la-creperie.jpg",
                19 => "caribbean-flavors.jpg",
                20 => "mama-africa.jpg",
                _ => "default.svg"
            };
        }

        public string RestaurantImageUrl
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Restaurant.ImageUrl))
                {
                    return $"/images/restaurants/{GetImageFileName(Restaurant.Id)}";
                }

                return Restaurant.ImageUrl;
            }
        }

        public bool HasCulturalStory
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Restaurant.CulturalStory) ||
                       !string.IsNullOrWhiteSpace(Restaurant.CulturalTraditions);
            }
        }

        public bool HasDietaryTags
        {
            get
            {
                return Restaurant.DietaryTags != null && Restaurant.DietaryTags.Any();
            }
        }

        public bool HasSignatureDishes
        {
            get
            {
                return Restaurant.SignatureDishes != null && Restaurant.SignatureDishes.Any();
            }
        }

        public string PriceLevel => "$$";

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Restaurant = await _db.Restaurants
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id) ?? new Restaurant();

            if (Restaurant.Id == 0)
            {
                return Redirect("/Error");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            RestaurantFeedback = await _db.Feedback
                .AsNoTracking()
                .Where(f => f.RestaurantId == id &&
                    (
                        f.Status == "Approved" ||
                        (!string.IsNullOrEmpty(userId) && f.UserId == userId)
                    ))
                .OrderByDescending(f => f.Id)
                .ToListAsync();

            return Page();
        }
    }
}