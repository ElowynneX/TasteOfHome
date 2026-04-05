using TasteOfHome.Models;

namespace TasteOfHome.Data
{
    public static class DbSeeder
    {
        public static void Seed(AppDbContext db)
        {
            SeedRestaurantsAndFeedback(db);
            SeedEvents(db);
        }

        private static void SeedRestaurantsAndFeedback(AppDbContext db)
        {
            if (db.Restaurants.Any())
                return;

            var seededRestaurants = RestaurantSeed.GetRestaurants();
            var seededFeedback = FeedbackSeed.GetFeedbacks();

            foreach (var restaurant in seededRestaurants)
            {
                var relevantFeedback = seededFeedback
                    .Where(f => f.RestaurantId == restaurant.Id)
                    .ToList();

                float averageRating = 0;
                int averageAuthenticity = 0;

                foreach (var feedback in relevantFeedback)
                {
                    averageRating += feedback.Rating;
                    averageAuthenticity += feedback.Authenticity;
                }

                db.Restaurants.Add(new Restaurant
                {
                    Name = restaurant.Name,
                    Cuisine = restaurant.Cuisine,
                    Location = restaurant.Location,
                    Address = restaurant.Address,
                    DietaryTags = restaurant.DietaryTags,
                    CulturalStory = restaurant.CulturalStory,
                    CulturalTraditions = restaurant.CulturalTraditions,
                    SignatureDishesCsv = restaurant.SignatureDishesCsv,
                    Rating = MathF.Round(averageRating / relevantFeedback.Count, 1),
                    Authenticity = averageAuthenticity / relevantFeedback.Count,
                    NumberOfReviews = relevantFeedback.Count,
                    ImageUrl = restaurant.ImageUrl
                });
            }

            foreach (var feedback in seededFeedback)
            {
                db.Feedback.Add(new Feedback
                {
                    Rating = feedback.Rating,
                    Authenticity = feedback.Authenticity,
                    Review = feedback.Review,
                    RestaurantId = feedback.RestaurantId
                });
            }

            db.SaveChanges();
        }

        private static void SeedEvents(AppDbContext db)
        {
            if (db.CulturalEvents.Any())
                return;

            var events = EventSeed.GetEvents();
            db.CulturalEvents.AddRange(events);
            db.SaveChanges();
        }
    }
}