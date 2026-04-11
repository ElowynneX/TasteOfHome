using TasteOfHome.Models;

namespace TasteOfHome.Services
{
    public interface IAiRecommendationService
    {
        Task<AiRecommendationResult> GetRecommendationsAsync(string prompt, List<Restaurant> restaurants);
    }

    public class AiRecommendationResult
    {
        public string Intro { get; set; } = "";
        public List<AiRecommendationItem> Suggestions { get; set; } = new();
    }

    public class AiRecommendationItem
    {
        public int RestaurantId { get; set; }
        public string Name { get; set; } = "";
        public string Reason { get; set; } = "";
    }
}