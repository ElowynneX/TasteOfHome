using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TasteOfHome.Models;

namespace TasteOfHome.Pages.Restaurants
{
    public class FeedbackModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public FeedbackModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        //Variable used in the page view
        public Restaurant Restaurant { get; set; } = new Restaurant();
        [BindProperty]
        public Feedback Feedback { get; set; } = new Feedback();


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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                var innerResponse = await _httpClient.GetAsync($"https://localhost:7024/api/Restaurants/{Feedback.RestaurantId}");
                if (!innerResponse.IsSuccessStatusCode)
                {
                    return Redirect("/Error");
                }
                Restaurant = await innerResponse.Content.ReadFromJsonAsync<Restaurant>();

                Console.WriteLine("FAILED");
                return Page();
            }

            var feedbackDto = new
            {
                Rating = Feedback.Rating,
                Authenticity = Feedback.Authenticity,
                Review = Feedback.Review,
                RestaurantId = Feedback.RestaurantId
            };

            var response = await _httpClient.PostAsJsonAsync("https://localhost:7024/api/Restaurants", feedbackDto);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Unable to create restaurant rating.");
                return Page();
            }

            return Redirect("/Restaurants");
        }
    }
}
