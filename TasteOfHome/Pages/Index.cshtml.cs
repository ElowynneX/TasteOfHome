using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TasteOfHome.Data;
using TasteOfHome.Models;

namespace TasteOfHome.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly AppDbContext _db;

        public IndexModel(ILogger<IndexModel> logger, AppDbContext db)
        {
            _logger = logger;
            _db = db;
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