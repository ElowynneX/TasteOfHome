using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TasteOfHome.Models
{
    public class CulturalEvent
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Title { get; set; } = "";

        [Required]
        [MaxLength(100)]
        public string Category { get; set; } = "";

        [Required]
        [MaxLength(100)]
        public string CultureTag { get; set; } = "";

        [Required]
        [MaxLength(200)]
        public string VenueName { get; set; } = "";

        [Required]
        [MaxLength(200)]
        public string Address { get; set; } = "";

        [Required]
        [MaxLength(100)]
        public string City { get; set; } = "";

        [Required]
        public DateTime EventDate { get; set; }

        [Required]
        [MaxLength(20)]
        public string StartTime { get; set; } = "";

        [Required]
        [MaxLength(20)]
        public string EndTime { get; set; } = "";

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; } = "";

        [MaxLength(1000)]
        public string? Highlights { get; set; }

        [MaxLength(500)]
        public string? DressCode { get; set; }

        [MaxLength(500)]
        public string? FoodDetails { get; set; }

        [MaxLength(500)]
        public string? EntertainmentDetails { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PricePerPerson { get; set; }

        public int Capacity { get; set; }

        public int ReservedSpots { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsFeatured { get; set; } = false;

        [MaxLength(100)]
        public string HostedBy { get; set; } = "TasteOfHome Admin";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [NotMapped]
        public int AvailableSpots => Math.Max(0, Capacity - ReservedSpots);

        [NotMapped]
        public bool IsSoldOut => AvailableSpots <= 0;

        public ICollection<EventReservation> Reservations { get; set; } = new List<EventReservation>();
    }
}