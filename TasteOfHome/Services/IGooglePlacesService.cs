namespace TasteOfHome.Services
{
    public interface IGooglePlacesService
    {
        Task<List<ImportedRestaurantDto>> SearchRestaurantsAsync(string query);
    }

    public class ImportedRestaurantDto
    {
        public string ExternalId { get; set; } = "";
        public string Name { get; set; } = "";
        public string Address { get; set; } = "";
        public string City { get; set; } = "";
        public string PostalCode { get; set; } = "";
        public string Cuisine { get; set; } = "restaurant";
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public string ImageUrl { get; set; } = "";

        public bool ServesVegetarianFood { get; set; } = false;
    }
}