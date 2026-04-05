using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TasteOfHome.Models
{
    public class EventReservation
    {
        public int Id { get; set; }

        [Required]
        public int CulturalEventId { get; set; }

        [ForeignKey(nameof(CulturalEventId))]
        public CulturalEvent? CulturalEvent { get; set; }

        [Required]
        public string UserId { get; set; } = "";

        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; } = "";

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = "";

        [Required]
        [StringLength(25)]
        public string PhoneNumber { get; set; } = "";

        [Range(1, 6)]
        public int NumberOfSpots { get; set; } = 1;

        [StringLength(500)]
        public string? Notes { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "PendingPayment";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "decimal(10,2)")]
        public decimal AmountPaid { get; set; }

        [StringLength(20)]
        public string PaymentStatus { get; set; } = "Pending";

        [StringLength(150)]
        public string? StripeCheckoutSessionId { get; set; }

        [StringLength(150)]
        public string? StripePaymentIntentId { get; set; }
    }
}