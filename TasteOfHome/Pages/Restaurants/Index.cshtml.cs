using Microsoft.AspNetCore.Mvc.RazorPages;
using TasteOfHome.Models;
using TasteOfHome.Data;
using Microsoft.AspNetCore.Mvc;

namespace TasteOfHome.Pages.Restaurants
{
    public class IndexModel : PageModel
    {
        public List<Restaurant> Restaurants { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public List<String> SelectedDietaryFilters{ get; set; } = new();

        //public List<string> DietaryTagList = new List<string> { "Vegetarian", "Vegan", "Halal", "Gluten-Free" };

        public void OnGet()
        {
            var allRestaurants = RestaurantSeed.GetRestaurants();

            if (SelectedDietaryFilters.Any())
            {
                Restaurants = allRestaurants
                    .Where(r => SelectedDietaryFilters.All(filter => r.DietaryTags.Contains(filter)))
                    .ToList();
            }
            else
            {
                Restaurants = allRestaurants;
            }
        }
    }
}
