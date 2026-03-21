using Microsoft.AspNetCore.Mvc.RazorPages;
using TasteOfHome.Models;
using TasteOfHome.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TasteOfHome.Pages.Restaurants
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _db;

        public IndexModel(AppDbContext db)
        {
            _db = db;
        }

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
                _ => "default.jpg"
            };
        }

        public List<Restaurant> Restaurants { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public List<string> SelectedDietaryFilters { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public List<string> SelectedCuisineFilter { get; set; } = new();

        public async Task OnGetAsync()
        {
            var filteredRestaurants = _db.Restaurants
                .AsNoTracking()
                .AsQueryable();

            if (SelectedCuisineFilter != null && SelectedCuisineFilter.Any())
            {
                filteredRestaurants = filteredRestaurants.Where(r =>
                    SelectedCuisineFilter.Contains(r.Cuisine));
            }

            var restaurants = await filteredRestaurants.ToListAsync();

            if (SelectedDietaryFilters != null && SelectedDietaryFilters.Any())
            {
                restaurants = restaurants
                    .Where(r => SelectedDietaryFilters.All(filter =>
                        r.DietaryTags.Any(tag =>
                            string.Equals(tag, filter, StringComparison.OrdinalIgnoreCase))))
                    .ToList();
            }

            Restaurants = restaurants;
        }
    }
}