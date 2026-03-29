using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TasteOfHome.Data;
using TasteOfHome.Models;
using TasteOfHome.Services;

namespace TasteOfHome.Pages
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _db;
        private readonly IGooglePlacesService _googlePlacesService;
        private readonly ILogger<IndexModel> _logger;
        private readonly IAiRestaurantEnrichmentService _aiRestaurantEnrichmentService;

        public IndexModel(
      AppDbContext db,
      IGooglePlacesService googlePlacesService,
      IAiRestaurantEnrichmentService aiRestaurantEnrichmentService)
        {
            _db = db;
            _googlePlacesService = googlePlacesService;
            _aiRestaurantEnrichmentService = aiRestaurantEnrichmentService;
        }

        public List<HiddenGem> ApprovedHiddenGems { get; set; } = new();

        public async Task OnGetAsync()
        {
            ApprovedHiddenGems = await _db.HiddenGems
                .AsNoTracking()
                .Where(h => h.Status == "Approved")
                .OrderByDescending(h => h.CreatedAt)
                .Take(6)
                .ToListAsync();
        }
    }
}