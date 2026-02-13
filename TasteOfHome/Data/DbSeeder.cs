using TasteOfHome.Models;

namespace TasteOfHome.Data
{
    public static class DbSeeder
    {
        public static void Seed(AppDbContext db)
        {
            if (db.Restaurants.Any()) return;

            var seeded = RestaurantSeed.GetRestaurants();

            foreach (var r in seeded)
            {
                db.Restaurants.Add(new Restaurant
                {
                    Name = r.Name,
                    Cuisine = r.Cuisine,
                    Location = r.Location,
                    DietaryTags = r.DietaryTags
                });
            }

            db.SaveChanges();
        }
    }
}
