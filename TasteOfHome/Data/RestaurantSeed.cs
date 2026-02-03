using TasteOfHome.Models;

namespace TasteOfHome.Data
{
    public static class RestaurantSeed
    {
        public static List<Restaurant> GetRestaurants()
        {
            return new List<Restaurant>
            {
                new Restaurant
                {
                    Id = 1,
                    Name = "Spice Garden",
                    Cuisine = "Indian",
                    Location = "Toronto",
                    DietaryTags = new List<string> { "Halal", "Vegetarian" }
                },
                new Restaurant
                {
                    Id = 2,
                    Name = "Green Bowl",
                    Cuisine = "Vegan",
                    Location = "Waterloo",
                    DietaryTags = new List<string> { "Vegan", "Gluten-Free" }
                },
                new Restaurant
                {
                    Id = 3,
                    Name = "Golden Wok",
                    Cuisine = "Chinese",
                    Location = "Markham",
                    DietaryTags = new List<string> { "Vegetarian" }
                },
                new Restaurant
                {
                    Id = 4,
                    Name = "Istanbul Grill",
                    Cuisine = "Turkish",
                    Location = "Mississauga",
                    DietaryTags = new List<string> { "Halal" }
                },
                new Restaurant
                {
                    Id = 5,
                    Name = "Nonna’s Kitchen",
                    Cuisine = "Italian",
                    Location = "Toronto",
                    DietaryTags = new List<string> { "Vegetarian" }
                },
                new Restaurant
                {
                    Id = 6,
                    Name = "Seoul Street",
                    Cuisine = "Korean",
                    Location = "North York",
                    DietaryTags = new List<string> { }
                },
                new Restaurant
                {
                    Id = 7,
                    Name = "Pho Saigon",
                    Cuisine = "Vietnamese",
                    Location = "Scarborough",
                    DietaryTags = new List<string> { "Halal", "Gluten-Free" }
                },
                new Restaurant
                {
                    Id = 8,
                    Name = "Tokyo Bento",
                    Cuisine = "Japanese",
                    Location = "Toronto",
                    DietaryTags = new List<string> { }
                },
                new Restaurant
                {
                    Id = 9,
                    Name = "El Mariachi",
                    Cuisine = "Mexican",
                    Location = "Brampton",
                    DietaryTags = new List<string> { "Gluten-Free" }
                },
                new Restaurant
                {
                    Id = 10,
                    Name = "Falafel House",
                    Cuisine = "Middle Eastern",
                    Location = "Toronto",
                    DietaryTags = new List<string> { "Halal", "Vegetarian", "Vegan" }
                },
                new Restaurant
                {
                    Id = 11,
                    Name = "Taste of Punjab",
                    Cuisine = "Indian",
                    Location = "Brampton",
                    DietaryTags = new List<string> { "Vegetarian" }
                },
                new Restaurant
                {
                    Id = 12,
                    Name = "Bangkok Express",
                    Cuisine = "Thai",
                    Location = "Toronto",
                    DietaryTags = new List<string> { "Gluten-Free" }
                },
                new Restaurant
                {
                    Id = 13,
                    Name = "Habesha Table",
                    Cuisine = "Ethiopian",
                    Location = "Toronto",
                    DietaryTags = new List<string> { "Vegetarian", "Vegan" }
                },
                new Restaurant
                {
                    Id = 14,
                    Name = "Casa Latina",
                    Cuisine = "Latin American",
                    Location = "Mississauga",
                    DietaryTags = new List<string> { }
                },
                new Restaurant
                {
                    Id = 15,
                    Name = "Mediterraneo",
                    Cuisine = "Greek",
                    Location = "Oakville",
                    DietaryTags = new List<string> { "Vegetarian" }
                },
                new Restaurant
                {
                    Id = 16,
                    Name = "Karachi BBQ",
                    Cuisine = "Pakistani",
                    Location = "Scarborough",
                    DietaryTags = new List<string> { "Halal" }
                },
                new Restaurant
                {
                    Id = 17,
                    Name = "Plant Power",
                    Cuisine = "Vegan",
                    Location = "Toronto",
                    DietaryTags = new List<string> { "Vegan", "Gluten-Free" }
                },
                new Restaurant
                {
                    Id = 18,
                    Name = "La Crêperie",
                    Cuisine = "French",
                    Location = "Toronto",
                    DietaryTags = new List<string> { "Vegetarian" }
                },
                new Restaurant
                {
                    Id = 19,
                    Name = "Caribbean Flavors",
                    Cuisine = "Caribbean",
                    Location = "Ajax",
                    DietaryTags = new List<string> { "Gluten-Free" }
                },
                new Restaurant
                {
                    Id = 20,
                    Name = "Mama Africa",
                    Cuisine = "African",
                    Location = "Toronto",
                    DietaryTags = new List<string> { "Halal" }
                }
            };
        }
    }
}
