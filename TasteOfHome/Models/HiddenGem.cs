using System;
using System.ComponentModel.DataAnnotations;

namespace TasteOfHome.Models
{
    public class HiddenGem
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Provider name is required")]
        [StringLength(120)]
        public string ProviderName { get; set; } = "";

        [Required(ErrorMessage = "Location is required")]
        [StringLength(120)]
        public string Location { get; set; } = "";

        // e.g., "Gujarati Tiffin", "Punjabi Home Chef"
        [Required(ErrorMessage = "Food type is required")]
        [StringLength(80)]
        public string FoodType { get; set; } = "";

        [Required(ErrorMessage = "Description is required")]
        [StringLength(600)]
        public string Description { get; set; } = "";

        // Optional contact (WhatsApp/phone/Instagram/email)
        [StringLength(120)]
        public string? ContactInfo { get; set; }

        // MVP: Pending or Approved
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending";

        // Optional hint: hidden gems often higher authenticity than restaurants
        [Range(0, 100, ErrorMessage = "Authenticity must be 0–100")]
        public int AuthenticityHint { get; set; } = 90;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}