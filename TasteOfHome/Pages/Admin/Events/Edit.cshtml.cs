using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TasteOfHome.Data;

namespace TasteOfHome.Pages.Admin.Events
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _configuration;

        public EditModel(AppDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            public int Id { get; set; }

            [Required]
            public string Title { get; set; } = "";

            [Required]
            public string Category { get; set; } = "";

            [Required]
            public string CultureTag { get; set; } = "";

            [Required]
            public string VenueName { get; set; } = "";

            [Required]
            public string Address { get; set; } = "";

            [Required]
            public string City { get; set; } = "";

            [Required]
            public DateTime? EventDate { get; set; }

            [Required]
            public string StartTime { get; set; } = "";

            [Required]
            public string EndTime { get; set; } = "";

            [Required]
            public string Description { get; set; } = "";

            public string? Highlights { get; set; }
            public string? DressCode { get; set; }
            public string? FoodDetails { get; set; }
            public string? EntertainmentDetails { get; set; }
            public string? ImageUrl { get; set; }

            [Required]
            [Range(0, 10000)]
            public decimal PricePerPerson { get; set; }

            [Required]
            [Range(1, 10000)]
            public int Capacity { get; set; }

            public string HostedBy { get; set; } = "TasteOfHome Admin";
            public bool IsActive { get; set; } = true;
            public bool IsFeatured { get; set; } = false;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (!IsAdminUser())
                return RedirectToPage("/Index");

            var ev = await _db.CulturalEvents.FindAsync(id);
            if (ev == null)
                return NotFound();

            // Load existing values into the form
            Input = new InputModel
            {
                Id = ev.Id,
                Title = ev.Title,
                Category = ev.Category,
                CultureTag = ev.CultureTag,
                VenueName = ev.VenueName,
                Address = ev.Address,
                City = ev.City,
                EventDate = ev.EventDate,
                StartTime = ev.StartTime,
                EndTime = ev.EndTime,
                Description = ev.Description,
                Highlights = ev.Highlights,
                DressCode = ev.DressCode,
                FoodDetails = ev.FoodDetails,
                EntertainmentDetails = ev.EntertainmentDetails,
                ImageUrl = ev.ImageUrl,
                PricePerPerson = ev.PricePerPerson,
                Capacity = ev.Capacity,
                HostedBy = ev.HostedBy,
                IsActive = ev.IsActive,
                IsFeatured = ev.IsFeatured
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!IsAdminUser())
                return RedirectToPage("/Index");

            if (!ModelState.IsValid)
                return Page();

            var ev = await _db.CulturalEvents.FindAsync(Input.Id);
            if (ev == null)
                return NotFound();

            // Apply changes
            ev.Title = Input.Title;
            ev.Category = Input.Category;
            ev.CultureTag = Input.CultureTag;
            ev.VenueName = Input.VenueName;
            ev.Address = Input.Address;
            ev.City = Input.City;
            ev.EventDate = Input.EventDate!.Value;
            ev.StartTime = Input.StartTime;
            ev.EndTime = Input.EndTime;
            ev.Description = Input.Description;
            ev.Highlights = Input.Highlights;
            ev.DressCode = Input.DressCode;
            ev.FoodDetails = Input.FoodDetails;
            ev.EntertainmentDetails = Input.EntertainmentDetails;
            ev.ImageUrl = Input.ImageUrl;
            ev.PricePerPerson = Input.PricePerPerson;
            ev.Capacity = Input.Capacity;
            ev.HostedBy = Input.HostedBy;
            ev.IsActive = Input.IsActive;
            ev.IsFeatured = Input.IsFeatured;

            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Event '{ev.Title}' updated successfully.";
            return RedirectToPage("/Admin/Events/Index");
        }

        private bool IsAdminUser()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var adminEmails = _configuration
                .GetSection("AdminSettings:AdminEmails")
                .Get<string[]>() ?? Array.Empty<string>();

            return User.Identity?.IsAuthenticated == true
                   && !string.IsNullOrWhiteSpace(userEmail)
                   && adminEmails.Any(a => string.Equals(a, userEmail, StringComparison.OrdinalIgnoreCase));
        }
    }
}