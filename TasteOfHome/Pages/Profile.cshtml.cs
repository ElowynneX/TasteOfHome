using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TasteOfHome.Data;

namespace TasteOfHome.Pages
{
    [Authorize]
    public class ProfileModel : PageModel
    {
        private readonly AppDbContext _db;

        public ProfileModel(AppDbContext db)
        {
            _db = db;
        }

        public string Email { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string DisplayInitial { get; set; } = "U";
        public string LocationLabel { get; set; } = "TasteOfHome Member";

        public int RestaurantReservationCount { get; set; }
        public int EventBookingCount { get; set; }
        public int HiddenGemSubmissionCount { get; set; }

        public List<ProfileListItem> RecentReservations { get; set; } = new();
        public List<ProfileListItem> RecentEventBookings { get; set; } = new();
        public List<ProfileListItem> RecentHiddenGems { get; set; } = new();

        public class ProfileListItem
        {
            public string Title { get; set; } = "";
            public string Subtitle { get; set; } = "";
        }

        public async Task OnGetAsync()
        {
            Email = User.Identity?.Name ?? "";

            if (string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(FakeUsers.LoggedInEmail))
            {
                Email = FakeUsers.LoggedInEmail;
            }

            if (string.IsNullOrWhiteSpace(Email))
            {
                return;
            }

            DisplayName = BuildDisplayName(Email);
            DisplayInitial = DisplayName.Substring(0, 1).ToUpper();

            // Safe defaults until reservation/event ownership fields are confirmed
            RestaurantReservationCount = 0;
            EventBookingCount = 0;

            HiddenGemSubmissionCount = await _db.HiddenGems
                .CountAsync(h => h.SubmittedByEmail == Email);

            RecentReservations = new List<ProfileListItem>();

            RecentEventBookings = new List<ProfileListItem>();

            RecentHiddenGems = await _db.HiddenGems
                .Where(h => h.SubmittedByEmail == Email)
                .OrderByDescending(h => h.Id)
                .Take(5)
                .Select(h => new ProfileListItem
                {
                    Title = h.ProviderName,
                    Subtitle = $"{h.Location} • {h.Status}"
                })
                .ToListAsync();
        }

        private static string BuildDisplayName(string email)
        {
            var localPart = email.Split('@')[0];
            if (string.IsNullOrWhiteSpace(localPart))
                return "User";

            var parts = localPart
                .Replace(".", " ")
                .Replace("_", " ")
                .Replace("-", " ")
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => char.ToUpper(p[0]) + p.Substring(1).ToLower());

            var fullName = string.Join(" ", parts);
            return string.IsNullOrWhiteSpace(fullName) ? "User" : fullName;
        }
    }
}