using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TasteOfHome.Data;
using TasteOfHome.Models;
using TasteOfHome.Services;

namespace TasteOfHome.Pages.Events
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _db;
        private readonly ILiveEventsService _liveEventsService;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(
            AppDbContext db,
            ILiveEventsService liveEventsService,
            ILogger<IndexModel> logger)
        {
            _db = db;
            _liveEventsService = liveEventsService;
            _logger = logger;
        }

        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? City { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Category { get; set; }

        [BindProperty(SupportsGet = true)]
        public double? Latitude { get; set; }

        [BindProperty(SupportsGet = true)]
        public double? Longitude { get; set; }

        public List<string> Categories { get; set; } = new();
        public List<CulturalEvent> HostedEvents { get; set; } = new();
        public List<LiveEvent> LiveEvents { get; set; } = new();

        public async Task OnGetAsync()
        {
            var query = _db.CulturalEvents
                .Where(e => e.IsActive)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(Search))
            {
                var searchLower = Search.ToLower();

                query = query.Where(e =>
                    e.Title.ToLower().Contains(searchLower) ||
                    e.Description.ToLower().Contains(searchLower) ||
                    e.City.ToLower().Contains(searchLower) ||
                    e.Category.ToLower().Contains(searchLower) ||
                    e.CultureTag.ToLower().Contains(searchLower));
            }

            if (!string.IsNullOrWhiteSpace(City))
            {
                var cityLower = City.ToLower();
                query = query.Where(e => e.City.ToLower().Contains(cityLower));
            }

            if (!string.IsNullOrWhiteSpace(Category))
            {
                var categoryLower = Category.ToLower();
                query = query.Where(e => e.Category.ToLower() == categoryLower);
            }

            Categories = await _db.CulturalEvents
                .Where(e => e.IsActive)
                .Select(e => e.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            HostedEvents = await query
                .OrderBy(e => e.EventDate)
                .ThenBy(e => e.StartTime)
                .ToListAsync();

            try
            {
                LiveEvents = await _liveEventsService.SearchEventsAsync(
                    Search,
                    City,
                    Category,
                    Latitude,
                    Longitude);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load live events from Ticketmaster.");
                LiveEvents = new List<LiveEvent>();
            }
        }
    }
}