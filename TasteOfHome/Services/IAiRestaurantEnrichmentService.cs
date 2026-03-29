namespace TasteOfHome.Services
{
    public interface IAiRestaurantEnrichmentService
    {
        Task<RestaurantEnrichmentResult> EnrichRestaurantAsync(
            string name,
            string cuisine,
            string city,
            string address,
            string existingDietaryTagsCsv);
    }

    public class RestaurantEnrichmentResult
    {
        public string CulturalStory { get; set; } = "";
        public string CulturalTraditions { get; set; } = "";
        public List<string> SignatureDishes { get; set; } = new();
        public List<string> DietaryTags { get; set; } = new();
    }
}