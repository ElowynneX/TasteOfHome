using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TasteOfHome.Data;

namespace TasteOfHome.Pages.Admin.Analytics
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _db;

        public IndexModel(AppDbContext db)
        {
            _db = db;
        }

        public int TotalRestaurants { get; set; }
        public int TotalReservations { get; set; }
        public int TotalEvents { get; set; }
        public int TotalEventBookings { get; set; }
        public int ActiveEvents { get; set; }
        public int SoldOutEvents { get; set; }
        public int PendingHiddenGems { get; set; }

        public decimal EventRevenue { get; set; }

        public List<CityAnalyticsItem> TopCities { get; set; } = new();
        public List<EventAnalyticsItem> TopEvents { get; set; } = new();
        public List<RestaurantAnalyticsItem> TopRestaurants { get; set; } = new();

        public string CityChartLabelsJson { get; set; } = "[]";
        public string CityChartDataJson { get; set; } = "[]";
        public string EventChartLabelsJson { get; set; } = "[]";
        public string EventChartDataJson { get; set; } = "[]";
        public string StatusChartDataJson { get; set; } = "[]";

        public async Task OnGetAsync()
        {
            TotalRestaurants = await _db.Restaurants.CountAsync();
            TotalReservations = await _db.Reservations.CountAsync();
            TotalEvents = await _db.CulturalEvents.CountAsync();
            TotalEventBookings = await _db.EventReservations.SumAsync(x => (int?)x.NumberOfSpots) ?? 0;
            ActiveEvents = await _db.CulturalEvents.CountAsync(x => x.IsActive);
            SoldOutEvents = await _db.CulturalEvents.CountAsync(x => x.Capacity > 0 && x.ReservedSpots >= x.Capacity);
            PendingHiddenGems = await _db.HiddenGems.CountAsync(x => x.Status == "Pending");

            var paidEventAmounts = await _db.EventReservations
                .Where(x => x.PaymentStatus == "Paid")
                .Select(x => x.AmountPaid)
                .ToListAsync();

            EventRevenue = paidEventAmounts.Sum();

            var allEvents = await _db.CulturalEvents.ToListAsync();

            TopCities = allEvents
                .GroupBy(x => x.City)
                .Select(g => new CityAnalyticsItem
                {
                    City = g.Key,
                    EventCount = g.Count(),
                    ReservedSpots = g.Sum(x => x.ReservedSpots)
                })
                .OrderByDescending(x => x.ReservedSpots)
                .ThenByDescending(x => x.EventCount)
                .Take(5)
                .ToList();

            TopEvents = allEvents
                .OrderByDescending(x => x.ReservedSpots)
                .ThenByDescending(x => x.PricePerPerson)
                .Take(5)
                .Select(x => new EventAnalyticsItem
                {
                    Title = x.Title,
                    City = x.City,
                    ReservedSpots = x.ReservedSpots,
                    Capacity = x.Capacity,
                    Revenue = x.PricePerPerson * x.ReservedSpots
                })
                .ToList();

            TopRestaurants = await _db.Restaurants
                .OrderBy(x => x.Name)
                .Take(5)
                .Select(x => new RestaurantAnalyticsItem
                {
                    Name = x.Name,
                    City = x.City,
                    Cuisine = x.Cuisine
                })
                .ToListAsync();

            CityChartLabelsJson = JsonSerializer.Serialize(TopCities.Select(x => x.City));
            CityChartDataJson = JsonSerializer.Serialize(TopCities.Select(x => x.ReservedSpots));

            EventChartLabelsJson = JsonSerializer.Serialize(TopEvents.Select(x => x.Title));
            EventChartDataJson = JsonSerializer.Serialize(TopEvents.Select(x => x.ReservedSpots));

            var inactiveEvents = Math.Max(0, TotalEvents - ActiveEvents);
            StatusChartDataJson = JsonSerializer.Serialize(new[] { ActiveEvents, SoldOutEvents, inactiveEvents });
        }

        public class CityAnalyticsItem
        {
            public string City { get; set; } = "";
            public int EventCount { get; set; }
            public int ReservedSpots { get; set; }
        }

        public class EventAnalyticsItem
        {
            public string Title { get; set; } = "";
            public string City { get; set; } = "";
            public int ReservedSpots { get; set; }
            public int Capacity { get; set; }
            public decimal Revenue { get; set; }
        }

        public class RestaurantAnalyticsItem
        {
            public string Name { get; set; } = "";
            public string City { get; set; } = "";
            public string Cuisine { get; set; } = "";
        }
    }
}