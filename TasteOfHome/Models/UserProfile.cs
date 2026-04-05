using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TasteOfHome.Models
{
    public class UserProfile
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string Email { get; set; } = "";

        [Required]
        [MaxLength(100)]
        public string DisplayName { get; set; } = "";

        [MaxLength(100)]
        public string? Location { get; set; }
    }
}