using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TasteOfHome.Models
{
    public class UserSettings
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string Email { get; set; } = "";

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = "";

        [MaxLength(25)]
        public string? PhoneNumber { get; set; }

        public bool EmailNotificationsEnabled { get; set; } = true;

        public bool SmsNotificationsEnabled { get; set; } = true;

        public bool EventAnnouncementsEnabled { get; set; } = true;

        [Range(1, 12)]
        public int DefaultGuestCount { get; set; } = 2;

        [MaxLength(100)]
        public string? DietaryPreference { get; set; }

        [MaxLength(50)]
        public string? SeatingPreference { get; set; }

        public bool MarketingEmailsEnabled { get; set; } = true;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}