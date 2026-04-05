using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TasteOfHome.Data;
using TasteOfHome.Models;

namespace TasteOfHome.Pages.Admin.Events
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _configuration;

        public CreateModel(AppDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public bool IsAdmin { get; set; }

        public class InputModel
        {
            public string Title { get; set; } = "";
            public string Category { get; set; } = "";
            public string CultureTag { get; set; } = "";
            public string VenueName { get; set; } = "";
            public string Address { get; set; } = "";
            public string City { get; set; } = "";
            public DateTime? EventDate { get; set; }
            public string StartTime { get; set; } = "";
            public string EndTime { get; set; } = "";
            public string Description { get; set; } = "";
            public string? Highlights { get; set; }
            public string? DressCode { get; set; }
            public string? FoodDetails { get; set; }
            public string? EntertainmentDetails { get; set; }
            public string? ImageUrl { get; set; }
            public decimal PricePerPerson { get; set; }
            public int Capacity { get; set; }
            public string HostedBy { get; set; } = "TasteOfHome Admin";
            public bool IsActive { get; set; } = true;
            public bool IsFeatured { get; set; } = false;
        }

        public IActionResult OnGet()
        {
            if (!CheckAdmin())
                return RedirectToPage("/AccessDenied");

            Input.EventDate = DateTime.Today.AddDays(1);
            Input.IsActive = true;
            Input.IsFeatured = false;
            Input.HostedBy = "TasteOfHome Admin";

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!CheckAdmin())
                return RedirectToPage("/AccessDenied");

            ValidateInput();

            if (!ModelState.IsValid)
                return Page();

            var eventItem = new CulturalEvent
            {
                Title = Input.Title.Trim(),
                Category = Input.Category.Trim(),
                CultureTag = Input.CultureTag.Trim(),
                VenueName = Input.VenueName.Trim(),
                Address = Input.Address.Trim(),
                City = Input.City.Trim(),
                EventDate = Input.EventDate!.Value.Date,
                StartTime = Input.StartTime.Trim(),
                EndTime = Input.EndTime.Trim(),
                Description = Input.Description.Trim(),
                Highlights = string.IsNullOrWhiteSpace(Input.Highlights) ? null : Input.Highlights.Trim(),
                DressCode = string.IsNullOrWhiteSpace(Input.DressCode) ? null : Input.DressCode.Trim(),
                FoodDetails = string.IsNullOrWhiteSpace(Input.FoodDetails) ? null : Input.FoodDetails.Trim(),
                EntertainmentDetails = string.IsNullOrWhiteSpace(Input.EntertainmentDetails) ? null : Input.EntertainmentDetails.Trim(),
                ImageUrl = string.IsNullOrWhiteSpace(Input.ImageUrl) ? null : Input.ImageUrl.Trim(),
                PricePerPerson = Input.PricePerPerson,
                Capacity = Input.Capacity,
                ReservedSpots = 0,
                IsActive = Input.IsActive,
                IsFeatured = Input.IsFeatured,
                HostedBy = string.IsNullOrWhiteSpace(Input.HostedBy) ? "TasteOfHome Admin" : Input.HostedBy.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            _db.CulturalEvents.Add(eventItem);
            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = "Event created successfully.";
            return RedirectToPage("/Admin/Events/Index");
        }

        private bool CheckAdmin()
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;

            var adminEmails = _configuration
                .GetSection("AdminSettings:AdminEmails")
                .Get<string[]>() ?? Array.Empty<string>();

            IsAdmin = User.Identity?.IsAuthenticated == true
                      && !string.IsNullOrWhiteSpace(userEmail)
                      && adminEmails.Any(a => string.Equals(a, userEmail, StringComparison.OrdinalIgnoreCase));

            return IsAdmin;
        }

        private void ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(Input.Title))
                ModelState.AddModelError("Input.Title", "Title is required.");

            if (string.IsNullOrWhiteSpace(Input.Category))
                ModelState.AddModelError("Input.Category", "Category is required.");

            if (string.IsNullOrWhiteSpace(Input.CultureTag))
                ModelState.AddModelError("Input.CultureTag", "Culture tag is required.");

            if (string.IsNullOrWhiteSpace(Input.VenueName))
                ModelState.AddModelError("Input.VenueName", "Venue name is required.");

            if (string.IsNullOrWhiteSpace(Input.Address))
                ModelState.AddModelError("Input.Address", "Address is required.");

            if (string.IsNullOrWhiteSpace(Input.City))
                ModelState.AddModelError("Input.City", "City is required.");

            if (!Input.EventDate.HasValue)
                ModelState.AddModelError("Input.EventDate", "Event date is required.");
            else if (Input.EventDate.Value.Date < DateTime.Today)
                ModelState.AddModelError("Input.EventDate", "Event date cannot be in the past.");

            if (string.IsNullOrWhiteSpace(Input.StartTime))
                ModelState.AddModelError("Input.StartTime", "Start time is required.");

            if (string.IsNullOrWhiteSpace(Input.EndTime))
                ModelState.AddModelError("Input.EndTime", "End time is required.");

            if (string.IsNullOrWhiteSpace(Input.Description))
                ModelState.AddModelError("Input.Description", "Description is required.");

            if (Input.Capacity <= 0)
                ModelState.AddModelError("Input.Capacity", "Capacity must be greater than 0.");

            if (Input.PricePerPerson < 0)
                ModelState.AddModelError("Input.PricePerPerson", "Price cannot be negative.");
        }
    }
}