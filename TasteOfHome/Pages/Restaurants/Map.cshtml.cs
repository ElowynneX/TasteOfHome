using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TasteOfHome.Data;
using TasteOfHome.Models;
using TasteOfHome.Services;

namespace TasteOfHome.Pages.Restaurants
{
    public class MapModel : PageModel
    {
        private readonly AppDbContext _db;
        private readonly ILiveMapPlacesService _liveMapPlacesService;

        public MapModel(AppDbContext db, ILiveMapPlacesService liveMapPlacesService)
        {
            _db = db;
            _liveMapPlacesService = liveMapPlacesService;
        }

        public List<MapRestaurantVm> Restaurants { get; set; } = new();
        public List<string> CuisineGroups { get; set; } = new();
        public List<string> CityGroups { get; set; } = new();

        public async Task OnGetAsync()
        {
            Restaurants = await _db.Restaurants
                .AsNoTracking()
                .Where(r => r.Latitude.HasValue && r.Longitude.HasValue)
                .OrderByDescending(r => r.Rating)
                .ThenByDescending(r => r.Authenticity)
                .Select(r => new MapRestaurantVm
                {
                    Id = r.Id,
                    Name = r.Name,
                    Cuisine = string.IsNullOrWhiteSpace(r.Cuisine) ? "Restaurant" : r.Cuisine,
                    City = string.IsNullOrWhiteSpace(r.City) ? r.Location : r.City,
                    Location = r.Location,
                    Address = r.Address,
                    Rating = r.Rating,
                    Authenticity = r.Authenticity,
                    Latitude = r.Latitude!.Value,
                    Longitude = r.Longitude!.Value,
                    ImageUrl = r.ImageUrl,
                    DietaryTags = r.DietaryTags
                })
                .ToListAsync();

            CuisineGroups = Restaurants
                .Select(r => r.Cuisine?.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x)
                .ToList()!;

            CityGroups = Restaurants
                .Select(r => r.City?.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x)
                .ToList()!;
        }

        public async Task<JsonResult> OnGetNearbyAsync(double lat, double lng, double radius = 15000)
        {
            var places = await _liveMapPlacesService.SearchNearbyRestaurantsAsync(lat, lng, radius);
            return new JsonResult(new { places });
        }

        public async Task<JsonResult> OnPostImportLivePlaceAsync([FromBody] ImportLivePlaceRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Name))
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "Invalid place data."
                });
            }

            var normalizedName = request.Name.Trim();
            var normalizedAddress = (request.Address ?? "").Trim();

            var existingRestaurant = await _db.Restaurants
                .FirstOrDefaultAsync(r =>
                    r.Name.ToLower() == normalizedName.ToLower() &&
                    (r.Address ?? "").ToLower() == normalizedAddress.ToLower());

            if (existingRestaurant == null)
            {
                var city = ExtractCityFromAddress(normalizedAddress);
                var cuisine = MapPrimaryTypeToCuisine(request.PrimaryType);

                var newRestaurant = new Restaurant
                {
                    Name = normalizedName,
                    Cuisine = cuisine,
                    Location = string.IsNullOrWhiteSpace(city) ? "Nearby Area" : city,
                    City = string.IsNullOrWhiteSpace(city) ? "Nearby Area" : city,
                    Address = normalizedAddress,
                    Rating = request.Rating.HasValue ? (float)request.Rating.Value : 4.0f,
                    Authenticity = 80,
                    Latitude = request.Latitude,
                    Longitude = request.Longitude,
                    ImageUrl = string.IsNullOrWhiteSpace(request.ImageUrl) ? null : request.ImageUrl,
                    CulturalStory = "Imported from live nearby restaurant discovery on the Culture Explorer Map.",
                    CulturalTraditions = "This restaurant was added from live map discovery and can now be explored through TasteOfHome.",
                    DietaryTags = new List<string>(),
                    SignatureDishes = new List<string>()
                };

                _db.Restaurants.Add(newRestaurant);
                await _db.SaveChangesAsync();

                existingRestaurant = newRestaurant;
            }
            else if (string.IsNullOrWhiteSpace(existingRestaurant.ImageUrl) && !string.IsNullOrWhiteSpace(request.ImageUrl))
            {
                existingRestaurant.ImageUrl = request.ImageUrl;
                await _db.SaveChangesAsync();
            }

            return new JsonResult(new
            {
                success = true,
                restaurantId = existingRestaurant.Id,
                detailsUrl = Url.Page("/Restaurants/Details", new { id = existingRestaurant.Id }),
                reserveUrl = Url.Page("/Reservations/Create", new { restaurantId = existingRestaurant.Id })
            });
        }

        private static string ExtractCityFromAddress(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                return "Nearby Area";

            var knownCities = new[]
            {
                "Toronto", "Mississauga", "Brampton", "Scarborough", "North York",
                "Etobicoke", "Markham", "Oakville", "Ajax", "Kitchener",
                "Cambridge", "Waterloo", "Vaughan", "Richmond Hill"
            };

            foreach (var city in knownCities)
            {
                if (address.Contains(city, StringComparison.OrdinalIgnoreCase))
                    return city;
            }

            return "Nearby Area";
        }

        private static string MapPrimaryTypeToCuisine(string? primaryType)
        {
            if (string.IsNullOrWhiteSpace(primaryType))
                return "Restaurant";

            var value = primaryType.Replace("_", " ").Trim().ToLowerInvariant();

            return value switch
            {
                "thai_restaurant" => "Thai",
                "indian_restaurant" => "Indian",
                "pakistani_restaurant" => "Pakistani",
                "italian_restaurant" => "Italian",
                "chinese_restaurant" => "Chinese",
                "japanese_restaurant" => "Japanese",
                "korean_restaurant" => "Korean",
                "middle_eastern_restaurant" => "Middle Eastern",
                "mexican_restaurant" => "Mexican",
                _ => System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.Replace("_", " "))
            };
        }

        public class ImportLivePlaceRequest
        {
            public string Name { get; set; } = "";
            public string Address { get; set; } = "";
            public string PrimaryType { get; set; } = "";
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public double? Rating { get; set; }
            public string? ImageUrl { get; set; }
        }

        public class MapRestaurantVm
        {
            public int Id { get; set; }
            public string Name { get; set; } = "";
            public string Cuisine { get; set; } = "";
            public string City { get; set; } = "";
            public string Location { get; set; } = "";
            public string Address { get; set; } = "";
            public float Rating { get; set; }
            public int Authenticity { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public string? ImageUrl { get; set; }
            public List<string> DietaryTags { get; set; } = new();
        }
    }
}