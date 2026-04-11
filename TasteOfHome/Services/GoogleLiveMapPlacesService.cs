using System.Text;
using System.Text.Json;

namespace TasteOfHome.Services
{
    public class GoogleLiveMapPlacesService : ILiveMapPlacesService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public GoogleLiveMapPlacesService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<List<NearbyRestaurantPlace>> SearchNearbyRestaurantsAsync(
            double latitude,
            double longitude,
            double radiusMeters,
            CancellationToken cancellationToken = default)
        {
            var apiKey = _configuration["GooglePlaces:ApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return new List<NearbyRestaurantPlace>();
            }

            radiusMeters = Math.Clamp(radiusMeters, 1000, 50000);

            var requestBody = new
            {
                includedTypes = new[] { "restaurant" },
                maxResultCount = 20,
                locationRestriction = new
                {
                    circle = new
                    {
                        center = new
                        {
                            latitude,
                            longitude
                        },
                        radius = radiusMeters
                    }
                }
            };

            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://places.googleapis.com/v1/places:searchNearby");

            request.Headers.Add("X-Goog-Api-Key", apiKey);
            request.Headers.Add(
                "X-Goog-FieldMask",
                "places.id,places.displayName,places.location,places.formattedAddress,places.rating,places.primaryType,places.photos");

            request.Content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json");

            using var response = await _httpClient.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return new List<NearbyRestaurantPlace>();
            }

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            using var doc = JsonDocument.Parse(json);

            var results = new List<NearbyRestaurantPlace>();

            if (!doc.RootElement.TryGetProperty("places", out var placesElement) ||
                placesElement.ValueKind != JsonValueKind.Array)
            {
                return results;
            }

            foreach (var place in placesElement.EnumerateArray())
            {
                var id = place.TryGetProperty("id", out var idElement)
                    ? idElement.GetString() ?? ""
                    : "";

                var name = "";
                if (place.TryGetProperty("displayName", out var displayNameElement) &&
                    displayNameElement.TryGetProperty("text", out var textElement))
                {
                    name = textElement.GetString() ?? "";
                }

                var address = place.TryGetProperty("formattedAddress", out var addressElement)
                    ? addressElement.GetString() ?? ""
                    : "";

                var primaryType = place.TryGetProperty("primaryType", out var typeElement)
                    ? typeElement.GetString() ?? ""
                    : "restaurant";

                double placeLat = 0;
                double placeLng = 0;

                if (place.TryGetProperty("location", out var locationElement))
                {
                    if (locationElement.TryGetProperty("latitude", out var latElement))
                        placeLat = latElement.GetDouble();

                    if (locationElement.TryGetProperty("longitude", out var lngElement))
                        placeLng = lngElement.GetDouble();
                }

                double? rating = null;
                if (place.TryGetProperty("rating", out var ratingElement) &&
                    ratingElement.ValueKind == JsonValueKind.Number)
                {
                    rating = ratingElement.GetDouble();
                }

                string? photoName = null;
                if (place.TryGetProperty("photos", out var photosElement) &&
                    photosElement.ValueKind == JsonValueKind.Array &&
                    photosElement.GetArrayLength() > 0)
                {
                    var firstPhoto = photosElement[0];
                    if (firstPhoto.TryGetProperty("name", out var photoNameElement))
                    {
                        photoName = photoNameElement.GetString();
                    }
                }

                if (string.IsNullOrWhiteSpace(name) || placeLat == 0 || placeLng == 0)
                    continue;

                string? imageUrl = null;
                if (!string.IsNullOrWhiteSpace(photoName))
                {
                    imageUrl = await TryGetPhotoUriAsync(apiKey, photoName, cancellationToken);
                }

                results.Add(new NearbyRestaurantPlace
                {
                    Id = id,
                    Name = name,
                    Address = address,
                    PrimaryType = primaryType,
                    Latitude = placeLat,
                    Longitude = placeLng,
                    Rating = rating,
                    PhotoName = photoName,
                    ImageUrl = imageUrl
                });
            }

            return results
                .GroupBy(x => x.Id)
                .Select(g => g.First())
                .ToList();
        }

        private async Task<string?> TryGetPhotoUriAsync(
            string apiKey,
            string photoName,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(photoName))
                return null;

            var encodedName = Uri.EscapeDataString(photoName);
            var url =
                $"https://places.googleapis.com/v1/{encodedName}/media?key={Uri.EscapeDataString(apiKey)}&maxWidthPx=800&skipHttpRedirect=true";

            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            using var response = await _httpClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            using var doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("photoUri", out var photoUriElement))
            {
                return photoUriElement.GetString();
            }

            return null;
        }
    }
}