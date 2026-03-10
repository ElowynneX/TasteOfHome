using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using TasteOfHome.Data;
using TasteOfHome.Models;

namespace TasteOfHome.Pages.Restaurants
{
    public class DetailsModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly AppDbContext _db;

        public DetailsModel(IHttpClientFactory httpClientFactory, AppDbContext db)
        {
            _httpClient = httpClientFactory.CreateClient();
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

        public Restaurant Restaurant { get; set; } = new Restaurant();
        public List<Feedback> RestaurantFeedback { get; set; } = new List<Feedback>();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var response = await _httpClient.GetAsync($"https://localhost:7024/api/Restaurants/{id}");

            if (!response.IsSuccessStatusCode)
            {
                return Redirect("/Error");
            }

            Restaurant = await response.Content.ReadFromJsonAsync<Restaurant>() ?? new Restaurant();

            RestaurantFeedback = await _db.Feedback
                .Where(f => f.RestaurantId == id &&
                           (f.Status == "Approved" || f.Status == null || f.Status == ""))
                .OrderByDescending(f => f.Id)
                .ToListAsync();

            return Page();
        }
    }
}