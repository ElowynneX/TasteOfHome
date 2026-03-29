using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TasteOfHome.Models
{
    public class Reservation
    {
        public int Id { get; set; }

        [Required]
        public int RestaurantId { get; set; }

        [ForeignKey(nameof(RestaurantId))]
        public Restaurant? Restaurant { get; set; }

        [Required]
        public string UserId { get; set; } = "";

        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; } = "";

        [Required]
        [StringLength(25)]
        public string PhoneNumber { get; set; } = "";

        [Required]
        public DateTime ReservationDate { get; set; }

        [Required]
        [StringLength(20)]
        public string ReservationTime { get; set; } = "";

        [Range(1, 12)]
        public int NumberOfGuests { get; set; } = 2;

        [StringLength(500)]
        public string? SpecialRequest { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}