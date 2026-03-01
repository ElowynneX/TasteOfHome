using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TasteOfHome.Data;
using TasteOfHome.Models;

namespace TasteOfHome.Pages.HiddenGems
{
    public class SubmitModel : PageModel
    {
        private readonly AppDbContext _db;

        public SubmitModel(AppDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public HiddenGem HiddenGem { get; set; } = new HiddenGem();

        [TempData]
        public string? SuccessMessage { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            HiddenGem.Status = "Pending";
            HiddenGem.CreatedAt = DateTime.UtcNow;

            _db.HiddenGems.Add(HiddenGem);
            await _db.SaveChangesAsync();

            SuccessMessage = "Hidden Gem submitted successfully! It is pending review.";
            return RedirectToPage("/HiddenGems/Index");
        }
    }
}