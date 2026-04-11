using Microsoft.EntityFrameworkCore;
using TasteOfHome.Data;
using TasteOfHome.Models;

namespace TasteOfHome.Services
{
    public class RestaurantCoordinateBackfillService
    {
        private readonly AppDbContext _db;

        public RestaurantCoordinateBackfillService(AppDbContext db)
        {
            _db = db;
        }

        public async Task BackfillAsync()
        {
            var restaurants = await _db.Restaurants.ToListAsync();
            var changed = false;

            foreach (var restaurant in restaurants)
            {
                if (restaurant.Latitude.HasValue && restaurant.Longitude.HasValue)
                    continue;

                var coords = GetCoordinatesForRestaurant(restaurant);

                if (coords != null)
                {
                    restaurant.Latitude = coords.Value.lat;
                    restaurant.Longitude = coords.Value.lng;
                    changed = true;
                }
            }

            if (changed)
            {
                await _db.SaveChangesAsync();
            }
        }

        private (double lat, double lng)? GetCoordinatesForRestaurant(Restaurant restaurant)
        {
            var key = (restaurant.Name ?? "").Trim().ToLowerInvariant();

            var map = new Dictionary<string, (double lat, double lng)>
            {
                ["spice garden"] = (43.6539, -79.3872),
                ["green bowl"] = (43.6629, -79.3957),
                ["golden wok"] = (43.6512, -79.4013),
                ["istanbul grill"] = (43.6489, -79.3942),
                ["nonna kitchen"] = (43.6457, -79.3891),
                ["seoul street"] = (43.6691, -79.3868),
                ["pho saigon"] = (43.6548, -79.3725),
                ["tokyo bento"] = (43.6482, -79.3819),
                ["el mariachi"] = (43.6503, -79.4102),
                ["falafel house"] = (43.6642, -79.4115),
                ["taste of punjab"] = (43.7006, -79.4393),
                ["bangkok express"] = (43.6714, -79.3879),
                ["habesha table"] = (43.6789, -79.3472),
                ["casa latina"] = (43.6478, -79.4201),
                ["mediterraneo"] = (43.6419, -79.4028),
                ["karachi bbq"] = (43.6941, -79.4515),
                ["plant power"] = (43.6569, -79.4097),
                ["la creperie"] = (43.6708, -79.3932),
                ["caribbean flavors"] = (43.6763, -79.4510),
                ["mama africa"] = (43.6882, -79.4335)
            };

            if (map.TryGetValue(key, out var coords))
                return coords;

            var location = (restaurant.Location ?? restaurant.City ?? "").Trim().ToLowerInvariant();

            if (location.Contains("toronto"))
                return RandomNearbyToronto(key);

            if (location.Contains("mississauga"))
                return RandomNearbyMississauga(key);

            if (location.Contains("scarborough"))
                return RandomNearbyScarborough(key);

            if (location.Contains("north york"))
                return RandomNearbyNorthYork(key);

            if (location.Contains("etobicoke"))
                return RandomNearbyEtobicoke(key);

            return RandomNearbyToronto(key);
        }

        private (double lat, double lng) RandomNearbyToronto(string seed)
        {
            return OffsetFromSeed(43.6532, -79.3832, seed);
        }

        private (double lat, double lng) RandomNearbyMississauga(string seed)
        {
            return OffsetFromSeed(43.5890, -79.6441, seed);
        }

        private (double lat, double lng) RandomNearbyScarborough(string seed)
        {
            return OffsetFromSeed(43.7731, -79.2578, seed);
        }

        private (double lat, double lng) RandomNearbyNorthYork(string seed)
        {
            return OffsetFromSeed(43.7615, -79.4111, seed);
        }

        private (double lat, double lng) RandomNearbyEtobicoke(string seed)
        {
            return OffsetFromSeed(43.6205, -79.5132, seed);
        }

        private (double lat, double lng) OffsetFromSeed(double baseLat, double baseLng, string seed)
        {
            var hash = Math.Abs(seed.GetHashCode());

            var latOffset = ((hash % 100) - 50) * 0.0012;
            var lngOffset = (((hash / 100) % 100) - 50) * 0.0012;

            return (baseLat + latOffset, baseLng + lngOffset);
        }
    }
}