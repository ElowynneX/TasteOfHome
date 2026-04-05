using System.ComponentModel.DataAnnotations;

namespace TasteOfHome.Models
{
    public class AdminSettings
    {
        public int Id { get; set; } = 1;

        public bool EnableRestaurantReservations { get; set; } = true;

        public bool EnableEventBookings { get; set; } = true;

        public bool EnableHiddenGemSubmissions { get; set; } = true;

        public bool RequireHiddenGemApproval { get; set; } = true;

        public bool ShowHiddenGemsOnHomepage { get; set; } = false;

        [Range(1, 12)]
        public int MaxGuestsPerReservation { get; set; } = 12;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}