using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using TasteOfHome.Models;

namespace TasteOfHome.Services
{
    public class GooglePlacesService : IGooglePlacesService
    {
        private readonly HttpClient _httpClient;
        private readonly GooglePlacesOptions _options;

        public GooglePlacesService(HttpClient httpClient, IOptions<GooglePlacesOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task<List<ImportedRestaurantDto>> SearchRestaurantsAsync(string query)
        {
            var url = $"{_options.BaseUrl}/places:searchText";

            var payload = new
            {
                textQuery = query,
                includedType = "restaurant",
                strictTypeFiltering = true,
                pageSize = 20
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, url);

            request.Headers.Add("X-Goog-Api-Key", _options.ApiKey);
            request.Headers.Add(
                "X-Goog-FieldMask",
                "places.id,places.displayName,places.formattedAddress,places.location,places.primaryType,places.types,places.addressComponents,places.photos,places.servesVegetarianFood"
            );

            request.Content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json");

            using var response = await _httpClient.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Google Places API error {(int)response.StatusCode}: {json}");
            }

            var results = new List<ImportedRestaurantDto>();

            using var doc = JsonDocument.Parse(json);

            if (!doc.RootElement.TryGetProperty("places", out var places) ||
                places.ValueKind != JsonValueKind.Array)
            {
                return results;
            }

            foreach (var place in places.EnumerateArray())
            {
                var dto = new ImportedRestaurantDto
                {
                    ExternalId = place.TryGetProperty("id", out var idProp)
                        ? idProp.GetString() ?? ""
                        : "",

                    Name = place.TryGetProperty("displayName", out var displayNameProp) &&
                           displayNameProp.TryGetProperty("text", out var textProp)
                        ? textProp.GetString() ?? ""
                        : "",

                    Address = place.TryGetProperty("formattedAddress", out var addressProp)
                        ? addressProp.GetString() ?? ""
                        : "",

                    Cuisine = place.TryGetProperty("primaryType", out var primaryTypeProp)
                        ? primaryTypeProp.GetString() ?? "restaurant"
                        : "restaurant"
                };

                if (string.IsNullOrWhiteSpace(dto.ExternalId) || string.IsNullOrWhiteSpace(dto.Name))
                    continue;

                bool looksLikeRestaurant = false;

                if (place.TryGetProperty("primaryType", out var primaryType))
                {
                    var primaryTypeValue = primaryType.GetString() ?? "";
                    if (primaryTypeValue.Contains("restaurant", StringComparison.OrdinalIgnoreCase))
                    {
                        looksLikeRestaurant = true;
                    }
                }

                if (!looksLikeRestaurant && place.TryGetProperty("types", out var typesProp))
                {
                    foreach (var t in typesProp.EnumerateArray())
                    {
                        var typeValue = t.GetString() ?? "";
                        if (typeValue.Contains("restaurant", StringComparison.OrdinalIgnoreCase))
                        {
                            looksLikeRestaurant = true;
                            break;
                        }
                    }
                }

                if (!looksLikeRestaurant)
                    continue;

                if (place.TryGetProperty("location", out var locationProp))
                {
                    if (locationProp.TryGetProperty("latitude", out var latProp) &&
                        latProp.TryGetDouble(out var latitude))
                    {
                        dto.Latitude = latitude;
                    }

                    if (locationProp.TryGetProperty("longitude", out var lngProp) &&
                        lngProp.TryGetDouble(out var longitude))
                    {
                        dto.Longitude = longitude;
                    }
                }

                if (place.TryGetProperty("addressComponents", out var componentsProp) &&
                    componentsProp.ValueKind == JsonValueKind.Array)
                {
                    foreach (var component in componentsProp.EnumerateArray())
                    {
                        if (!component.TryGetProperty("types", out var componentTypesProp) ||
                            componentTypesProp.ValueKind != JsonValueKind.Array)
                        {
                            continue;
                        }

                        var componentTypes = componentTypesProp
                            .EnumerateArray()
                            .Select(t => t.GetString() ?? "")
                            .ToList();

                        if (componentTypes.Contains("locality") &&
                            component.TryGetProperty("longText", out var cityProp))
                        {
                            dto.City = cityProp.GetString() ?? "";
                        }

                        if (componentTypes.Contains("postal_code") &&
                            component.TryGetProperty("longText", out var postalProp))
                        {
                            dto.PostalCode = postalProp.GetString() ?? "";
                        }
                    }
                }

                if (place.TryGetProperty("servesVegetarianFood", out var vegProp) &&
                    (vegProp.ValueKind == JsonValueKind.True || vegProp.ValueKind == JsonValueKind.False))
                {
                    dto.ServesVegetarianFood = vegProp.GetBoolean();
                }

                if (!string.IsNullOrWhiteSpace(dto.City) &&
                    dto.Name.Equals(dto.City, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (place.TryGetProperty("photos", out var photosProp) &&
                    photosProp.ValueKind == JsonValueKind.Array &&
                    photosProp.GetArrayLength() > 0)
                {
                    var firstPhoto = photosProp[0];

                    if (firstPhoto.TryGetProperty("name", out var photoNameProp))
                    {
                        var photoName = photoNameProp.GetString() ?? "";
                        if (!string.IsNullOrWhiteSpace(photoName))
                        {
                            dto.ImageUrl = await GetPhotoUrlAsync(photoName);
                        }
                    }
                }

                results.Add(dto);
            }

            return results;
        }

        private async Task<string> GetPhotoUrlAsync(string photoName)
        {
            var url = $"{_options.BaseUrl}/{photoName}/media?maxHeightPx=400&maxWidthPx=600&skipHttpRedirect=true";

            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("X-Goog-Api-Key", _options.ApiKey);

            using var response = await _httpClient.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return "";

            using var doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("photoUri", out var photoUriProp))
            {
                return photoUriProp.GetString() ?? "";
            }

            return "";
        }
    }
}