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

        //Create list of all Restaurants
        public List<Restaurant> Restaurants { get; set; } = new();

        //Set list for the selected dietary filters
        [BindProperty(SupportsGet = true)]
        public List<string> SelectedDietaryFilters { get; set; } = new();

        //Create another list for the selected location filter
        [BindProperty(SupportsGet = true)]
        public List<string> SelectedCuisineFilter { get; set; } = new();


        //--------------//
        //Page Function
        //--------------//
        public async Task OnGetAsync()
        {
            //Start with DB query instead of RestaurantSeed
            var filteredRestaurants = _db.Restaurants.AsNoTracking().AsQueryable();

            //Apply any dietary filters
            if (SelectedDietaryFilters.Any())
            {
                
                foreach (var filter in SelectedDietaryFilters)
                {
                    var f = filter.Trim();
                    filteredRestaurants = filteredRestaurants.Where(r => r.DietaryTagsCsv.Contains(f));
                }
            }

            //Apply any cuisine filters
            if (SelectedCuisineFilter.Any())
            {
                filteredRestaurants = filteredRestaurants.Where(r =>
                    SelectedCuisineFilter.Contains(r.Cuisine));
            }

            Restaurants = await filteredRestaurants.ToListAsync();
        }
    }
}
