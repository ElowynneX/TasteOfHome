namespace TasteOfHome.Services
{
    public interface ILiveMapPlacesService
    {
        Task<List<NearbyRestaurantPlace>> SearchNearbyRestaurantsAsync(
            double latitude,
            double longitude,
            double radiusMeters,
            CancellationToken cancellationToken = default);
    }

    public class NearbyRestaurantPlace
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Address { get; set; } = "";
        public string PrimaryType { get; set; } = "";
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? Rating { get; set; }

        public string? PhotoName { get; set; }
        public string? ImageUrl { get; set; }
    }
}