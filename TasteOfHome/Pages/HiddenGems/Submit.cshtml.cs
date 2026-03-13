using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TasteOfHome.Data;
using TasteOfHome.Models;

namespace TasteOfHome.Pages.HiddenGems
{
    [Authorize]
    public class SubmitModel : PageModel
    {
        private readonly AppDbContext _db;

        public SubmitModel(AppDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public HiddenGem HiddenGem { get; set; } = new HiddenGem();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            HiddenGem.Status = "Pending";
            HiddenGem.CreatedAt = DateTime.UtcNow;

            _db.HiddenGems.Add(HiddenGem);
            await _db.SaveChangesAsync();

            TempData["StatusMessage"] = "Hidden Gem submitted and is pending admin review.";
            return RedirectToPage("/Index");
        }
    }
}