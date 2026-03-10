using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TasteOfHome.Data;
using TasteOfHome.Models;

namespace TasteOfHome.Pages.Restaurants
{
    public class DetailsModel : PageModel
    {
        private readonly HttpClient _httpClient;
        public DetailsModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
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

        //Variable used in the page view
        public Restaurant Restaurant { get; set; }


        //--------------//
        //Page Function
        //--------------//
        public async Task<IActionResult> OnGetAsync(int id)
        {
            //Start by retrieving information from DB VIA API call
            var response = await _httpClient.GetAsync($"https://localhost:7024/api/Restaurants/{id}");

            //If the API call fails, redirect to ERROR page
            if (!response.IsSuccessStatusCode)
            {
                return Redirect("/Error");
            }

            //If the API call succeeds, get the body content from the API response
            Restaurant = await response.Content.ReadFromJsonAsync<Restaurant>();
            return Page();
        }
    }
}
