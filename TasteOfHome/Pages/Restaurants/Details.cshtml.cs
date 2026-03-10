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

        //Variable used in the page view
        public Restaurant Restaurant { get; set; }
        public List<Feedback> RestaurantFeedback { get; set; } = new List<Feedback>();


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

            //FUNCTION IS CURRENTLY UNFINISHED - COME BACK TO ME LATER
            var getFeedbackAttempt = await _httpClient.GetAsync($"https://localhost:7024/api/Restaurants/Feedback/{id}");

            if (!getFeedbackAttempt.IsSuccessStatusCode)
            {
                RestaurantFeedback = new List<Feedback>();
            }
            else
            {
                RestaurantFeedback = await getFeedbackAttempt.Content.ReadFromJsonAsync<List<Feedback>>()
                                     ?? new List<Feedback>();
            }

            //If the API call succeeds, get the body content from the API response
            Restaurant = await response.Content.ReadFromJsonAsync<Restaurant>();
            return Page();
        }
    }
}
