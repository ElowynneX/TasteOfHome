using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TasteOfHome.Models
{
    public class Feedback
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        [Range(0,5)]
        public int Rating { get; set; } = 0;

        [Required]
        [Range(0,100)]
        public int Authenticity { get; set; } = 0;

        [MaxLength(255)]
        public string Review { get; set; } = "";
        public int RestaurantId { get; set; } = 0;

        //Constructors
        public Feedback() { }

        public Feedback(int rating, int authenticity, string review, int restaurantId)
        {
            this.Rating = rating;
            this.Review = review;
            this.Authenticity = authenticity;
            this.RestaurantId = restaurantId;
        }
    }

    public class FeedbackDTO
    {
        public int RestaurantId { get; set; }
        public int Rating { get; set; }
        public int Authenticity { get; set; }
        public string Review { get; set; } = "N/A";
    }
}
