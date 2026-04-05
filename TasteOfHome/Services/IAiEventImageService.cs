using TasteOfHome.Models;

namespace TasteOfHome.Services
{
    public interface IAiEventImageService
    {
        Task<string?> GenerateEventImageAsync(EventImageGenerationRequest request, CancellationToken cancellationToken = default);
    }

    public class EventImageGenerationRequest
    {
        public string Title { get; set; } = "";
        public string Category { get; set; } = "";
        public string CultureTag { get; set; } = "";
        public string VenueName { get; set; } = "";
        public string City { get; set; } = "";
        public string Description { get; set; } = "";
        public string? FoodDetails { get; set; }
        public string? EntertainmentDetails { get; set; }
        public string? DressCode { get; set; }
    }
}