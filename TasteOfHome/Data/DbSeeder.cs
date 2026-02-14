using TasteOfHome.Models;

namespace TasteOfHome.Data
{
    public static class DbSeeder
    {
        public static void Seed(AppDbContext db)
        {
            //Abort if there are already restaurants in the DB
            if (db.Restaurants.Any()) return;

            //Set seeded information into lists for transfer into DB
            var seeded = RestaurantSeed.GetRestaurants();
            List<Feedback> feedbackSeeded = FeedbackSeed.GetFeedbacks();

            // =====================
            // Restaurant Seeding
            // =====================
            foreach (var r in seeded)
            {
                //Grab all feedback related to current restaurant
                List<Feedback> relevantFeedback = feedbackSeeded.Where(f => f.RestaurantId == r.Id).ToList();
                float aveRating = 0;
                int aveAuth = 0;

                //Calculate the average rating & authenticity based on seeded feedback
                foreach (var rf in relevantFeedback)
                {
                    aveRating += rf.Rating;
                    aveAuth += rf.Authenticity;
                }

                db.Restaurants.Add(new Restaurant
                {
                    Name = r.Name,
                    Cuisine = r.Cuisine,
                    Location = r.Location,
                    DietaryTags = r.DietaryTags,
                    Address = r.Address,
                    Rating = MathF.Round(aveRating / relevantFeedback.Count, 1),
                    Authenticity = aveAuth / relevantFeedback.Count,
                    NumberOfReviews = relevantFeedback.Count
                });
            }

            // =====================
            // Feedback Seeding (doesn't seed if there are restaurants in the DB)
            // =====================
            foreach (var f in feedbackSeeded)
            {
                db.Feedback.Add(new Feedback
                {
                    Rating = f.Rating,
                    Authenticity = f.Authenticity,
                    Review = f.Review,
                    RestaurantId = f.RestaurantId
                });
            }

            //Save changes to the DB
            db.SaveChanges();

            //Save changes to the DB
            db.SaveChanges();
        }
    }
}
