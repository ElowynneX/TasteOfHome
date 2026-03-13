using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TasteOfHome.Data;
using TasteOfHome.Models;

namespace TasteOfHome.Pages.HiddenGems
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _db;

        public IndexModel(AppDbContext db)
        {
            _db = db;
        }

        public List<HiddenGem> HiddenGems { get; set; } = new();

        public async Task OnGetAsync()
        {
            HiddenGems = await _db.HiddenGems
                .AsNoTracking()
                .Where(h => h.Status == "Approved")
                .OrderByDescending(h => h.CreatedAt)
                .ToListAsync();
        }
    }
}