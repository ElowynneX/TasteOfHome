using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TasteOfHome.Models
{
    public class HiddenGem
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string ProviderName { get; set; } = "";

        [Required]
        [MaxLength(100)]
        public string Location { get; set; } = "";

        [Required]
        [MaxLength(100)]
        public string FoodType { get; set; } = "";

        [Required]
        [MaxLength(500)]
        public string Description { get; set; } = "";

        [MaxLength(100)]
        public string? ContactInfo { get; set; }

        [Range(0, 100)]
        public int AuthenticityHint { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Pending";

        [Required(ErrorMessage = "Your phone number is required.")]
        [Display(Name = "Your Phone Number")]
        [MaxLength(20)]
        public string SubmitterPhoneNumber { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}