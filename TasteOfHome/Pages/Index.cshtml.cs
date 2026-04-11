using Microsoft.AspNetCore.Mvc;
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
        private readonly IAiRecommendationService _aiService;

        public IndexModel(AppDbContext db, IAiRecommendationService aiService)
        {
            _db = db;
            _aiService = aiService;
        }

        public List<HiddenGem> ApprovedHiddenGems { get; set; } = new();

        [BindProperty]
        public string AiPrompt { get; set; } = "";

        public async Task OnGetAsync()
        {
            var adminSettings = await _db.AdminSettings
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == 1);

            var showHiddenGems = adminSettings?.ShowHiddenGemsOnHomepage ?? false;

            if (showHiddenGems)
            {
                ApprovedHiddenGems = await _db.HiddenGems
                    .AsNoTracking()
                    .Where(h => h.Status == "Approved")
                    .OrderByDescending(h => h.CreatedAt)
                    .Take(3)
                    .ToListAsync();
            }
        }

        public async Task<JsonResult> OnPostAskAiAsync()
        {
            if (string.IsNullOrWhiteSpace(AiPrompt))
            {
                return new JsonResult(new
                {
                    intro = "Please tell me what kind of food you want.",
                    suggestions = new object[0]
                });
            }

            var restaurants = await _db.Restaurants
                .AsNoTracking()
                .ToListAsync();

            var result = await _aiService.GetRecommendationsAsync(AiPrompt.Trim(), restaurants);

            return new JsonResult(result);
        }
    }
}