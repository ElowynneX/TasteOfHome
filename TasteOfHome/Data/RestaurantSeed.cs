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
                    DietaryTags = new List<string> { "Halal", "Vegetarian" },
                    Address = "123 Queen St W, Toronto, ON"
                },
                new Restaurant
                {
                    Id = 2,
                    Name = "Green Bowl",
                    Cuisine = "Vegan",
                    Location = "Waterloo",
                    DietaryTags = new List<string> { "Vegan", "Gluten-Free" },
                    Address = "45 King St N, Waterloo, ON"
                },
                new Restaurant
                {
                    Id = 3,
                    Name = "Golden Wok",
                    Cuisine = "Chinese",
                    Location = "Markham",
                    DietaryTags = new List<string> { "Vegetarian" },
                    Address = "789 Highway 7, Markham, ON"
                },
                new Restaurant
                {
                    Id = 4,
                    Name = "Istanbul Grill",
                    Cuisine = "Turkish",
                    Location = "Mississauga",
                    DietaryTags = new List<string> { "Halal" },
                    Address = "310 Burnhamthorpe Rd, Mississauga, ON"
                },
                new Restaurant
                {
                    Id = 5,
                    Name = "Nonna’s Kitchen",
                    Cuisine = "Italian",
                    Location = "Toronto",
                    DietaryTags = new List<string> { "Vegetarian" },
                    Address = "88 College St, Toronto, ON"
                },
                new Restaurant
                {
                    Id = 6,
                    Name = "Seoul Street",
                    Cuisine = "Korean",
                    Location = "North York",
                    DietaryTags = new List<string> { },
                    Address = "512 Yonge St, North York, ON"
                },
                new Restaurant
                {
                    Id = 7,
                    Name = "Pho Saigon",
                    Cuisine = "Vietnamese",
                    Location = "Scarborough",
                    DietaryTags = new List<string> { "Halal", "Gluten-Free" },
                    Address = "401 Lawrence Ave E, Scarborough, ON"
                },
                new Restaurant
                {
                    Id = 8,
                    Name = "Tokyo Bento",
                    Cuisine = "Japanese",
                    Location = "Toronto",
                    DietaryTags = new List<string> { },
                    Address = "210 Dundas St W, Toronto, ON"
                },
                new Restaurant
                {
                    Id = 9,
                    Name = "El Mariachi",
                    Cuisine = "Mexican",
                    Location = "Brampton",
                    DietaryTags = new List<string> { "Gluten-Free" },
                    Address = "90 Main St S, Brampton, ON"
                },
                new Restaurant
                {
                    Id = 10,
                    Name = "Falafel House",
                    Cuisine = "Middle Eastern",
                    Location = "Toronto",
                    DietaryTags = new List<string> { "Halal", "Vegetarian", "Vegan" },
                    Address = "670 Bloor St W, Toronto, ON"
                },
                new Restaurant
                {
                    Id = 11,
                    Name = "Taste of Punjab",
                    Cuisine = "Indian",
                    Location = "Brampton",
                    DietaryTags = new List<string> { "Vegetarian" },
                    Address = "188 Queen St E, Brampton, ON"
                },
                new Restaurant
                {
                    Id = 12,
                    Name = "Bangkok Express",
                    Cuisine = "Thai",
                    Location = "Toronto",
                    DietaryTags = new List<string> { "Gluten-Free" },
                    Address = "455 Spadina Ave, Toronto, ON"
                },
                new Restaurant
                {
                    Id = 13,
                    Name = "Habesha Table",
                    Cuisine = "Ethiopian",
                    Location = "Toronto",
                    DietaryTags = new List<string> { "Vegetarian", "Vegan" },
                    Address = "337 Bloor St W, Toronto, ON"
                },
                new Restaurant
                {
                    Id = 14,
                    Name = "Casa Latina",
                    Cuisine = "Latin American",
                    Location = "Mississauga",
                    DietaryTags = new List<string> { },
                    Address = "205 Lakeshore Rd E, Mississauga, ON"
                },
                new Restaurant
                {
                    Id = 15,
                    Name = "Mediterraneo",
                    Cuisine = "Greek",
                    Location = "Oakville",
                    DietaryTags = new List<string> { "Vegetarian" },
                    Address = "135 Kerr St, Oakville, ON"
                },
                new Restaurant
                {
                    Id = 16,
                    Name = "Karachi BBQ",
                    Cuisine = "Pakistani",
                    Location = "Scarborough",
                    DietaryTags = new List<string> { "Halal" },
                    Address = "925 Warden Ave, Scarborough, ON"
                },
                new Restaurant
                {
                    Id = 17,
                    Name = "Plant Power",
                    Cuisine = "Vegan",
                    Location = "Toronto",
                    DietaryTags = new List<string> { "Vegan", "Gluten-Free" },
                    Address = "410 Queen St W, Toronto, ON"
                },
                new Restaurant
                {
                    Id = 18,
                    Name = "La Crêperie",
                    Cuisine = "French",
                    Location = "Toronto",
                    DietaryTags = new List<string> { "Vegetarian" },
                    Address = "260 King St W, Toronto, ON"
                },
                new Restaurant
                {
                    Id = 19,
                    Name = "Caribbean Flavors",
                    Cuisine = "Caribbean",
                    Location = "Ajax",
                    DietaryTags = new List<string> { "Gluten-Free" },
                    Address = "50 Bayly St W, Ajax, ON"
                },
                new Restaurant
                {
                    Id = 20,
                    Name = "Mama Africa",
                    Cuisine = "African",
                    Location = "Toronto",
                    DietaryTags = new List<string> { "Halal" },
                    Address = "777 Dundas St E, Toronto, ON"
                }
            };
        }
    }
}
