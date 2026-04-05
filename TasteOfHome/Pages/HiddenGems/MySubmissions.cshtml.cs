using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TasteOfHome.Data;
using TasteOfHome.Models;

namespace TasteOfHome.Pages.HiddenGems
{
    [Authorize]
    public class MySubmissionsModel : PageModel
    {
        private readonly AppDbContext _db;

        public MySubmissionsModel(AppDbContext db)
        {
            _db = db;
        }

        public List<HiddenGem> MyHiddenGems { get; set; } = new();

        public async Task OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = User.FindFirstValue(ClaimTypes.Email);

            MyHiddenGems = await _db.HiddenGems
                .AsNoTracking()
                .Where(h =>
                    (!string.IsNullOrWhiteSpace(userId) && h.UserId == userId) ||
                    (!string.IsNullOrWhiteSpace(email) && h.SubmittedByEmail == email))
                .OrderByDescending(h => h.CreatedAt)
                .ToListAsync();
        }
    }
}