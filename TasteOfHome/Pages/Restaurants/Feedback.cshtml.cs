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
        public Restaurant Restaurant { get; set; }


        //--------------//
        //Page Function
        //--------------//
        public void OnGet()
        {
        }
    }
}
