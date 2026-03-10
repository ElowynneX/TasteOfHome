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
            Address = "123 Queen St W, Toronto, ON",
            ImageUrl = "/images/restaurants/spice-garden.jpg",
            DietaryTags = new List<string> { "Halal", "Vegetarian" }
        },
        new Restaurant
        {
            Id = 2,
            Name = "Green Bowl",
            Cuisine = "Vegan",
            Location = "Waterloo",
            Address = "45 King St N, Waterloo, ON",
            ImageUrl = "/images/restaurants/green-bowl.jpg",
            DietaryTags = new List<string> { "Vegan", "Gluten-Free" }
        },
        new Restaurant
        {
            Id = 3,
            Name = "Golden Wok",
            Cuisine = "Chinese",
            Location = "Markham",
            Address = "789 Highway 7, Markham, ON",
            ImageUrl = "/images/restaurants/golden-wok.jpg",
            DietaryTags = new List<string> { "Vegetarian" }
        },
        new Restaurant
        {
            Id = 4,
            Name = "Istanbul Grill",
            Cuisine = "Turkish",
            Location = "Mississauga",
            Address = "310 Burnhamthorpe Rd, Mississauga, ON",
            ImageUrl = "/images/restaurants/istanbul-grill.jpg",
            DietaryTags = new List<string> { "Halal" }
        },
        new Restaurant
        {
            Id = 5,
            Name = "Nonna’s Kitchen",
            Cuisine = "Italian",
            Location = "Toronto",
            Address = "88 College St, Toronto, ON",
            ImageUrl = "/images/restaurants/nonna-kitchen.jpg",
            DietaryTags = new List<string> { "Vegetarian" }
        },
        new Restaurant
        {
            Id = 6,
            Name = "Seoul Street",
            Cuisine = "Korean",
            Location = "North York",
            Address = "512 Yonge St, North York, ON",
            ImageUrl = "/images/restaurants/seoul-street.jpg",
            DietaryTags = new List<string>()
        },
        new Restaurant
        {
            Id = 7,
            Name = "Pho Saigon",
            Cuisine = "Vietnamese",
            Location = "Scarborough",
            Address = "401 Lawrence Ave E, Scarborough, ON",
            ImageUrl = "/images/restaurants/pho-saigon.jpg",
            DietaryTags = new List<string> { "Halal", "Gluten-Free" }
        },
        new Restaurant
        {
            Id = 8,
            Name = "Tokyo Bento",
            Cuisine = "Japanese",
            Location = "Toronto",
            Address = "210 Dundas St W, Toronto, ON",
            ImageUrl = "/images/restaurants/tokyo-bento.jpg",
            DietaryTags = new List<string>()
        },
        new Restaurant
        {
            Id = 9,
            Name = "El Mariachi",
            Cuisine = "Mexican",
            Location = "Brampton",
            Address = "90 Main St S, Brampton, ON",
            ImageUrl = "/images/restaurants/el-mariachi.jpg",
            DietaryTags = new List<string> { "Gluten-Free" }
        },
        new Restaurant
        {
            Id = 10,
            Name = "Falafel House",
            Cuisine = "Middle Eastern",
            Location = "Toronto",
            Address = "670 Bloor St W, Toronto, ON",
            ImageUrl = "/images/restaurants/falafel-house.jpg",
            DietaryTags = new List<string> { "Halal", "Vegetarian", "Vegan" }
        },
        new Restaurant
        {
            Id = 11,
            Name = "Taste of Punjab",
            Cuisine = "Indian",
            Location = "Brampton",
            Address = "188 Queen St E, Brampton, ON",
            ImageUrl = "/images/restaurants/taste-of-punjab.jpg",
            DietaryTags = new List<string> { "Vegetarian" }
        },
        new Restaurant
        {
            Id = 12,
            Name = "Bangkok Express",
            Cuisine = "Thai",
            Location = "Toronto",
            Address = "455 Spadina Ave, Toronto, ON",
            ImageUrl = "/images/restaurants/bangkok-express.jpg",
            DietaryTags = new List<string> { "Gluten-Free" }
        },
        new Restaurant
        {
            Id = 13,
            Name = "Habesha Table",
            Cuisine = "Ethiopian",
            Location = "Toronto",
            Address = "337 Bloor St W, Toronto, ON",
            ImageUrl = "/images/restaurants/habesha-table.jpg",
            DietaryTags = new List<string> { "Vegetarian", "Vegan" }
        },
        new Restaurant
        {
            Id = 14,
            Name = "Casa Latina",
            Cuisine = "Latin American",
            Location = "Mississauga",
            Address = "205 Lakeshore Rd E, Mississauga, ON",
            ImageUrl = "/images/restaurants/casa-latina.jpg",
            DietaryTags = new List<string>()
        },
        new Restaurant
        {
            Id = 15,
            Name = "Mediterraneo",
            Cuisine = "Greek",
            Location = "Oakville",
            Address = "135 Kerr St, Oakville, ON",
            ImageUrl = "/images/restaurants/mediterraneo.jpg",
            DietaryTags = new List<string> { "Vegetarian" }
        },
        new Restaurant
        {
            Id = 16,
            Name = "Karachi BBQ",
            Cuisine = "Pakistani",
            Location = "Scarborough",
            Address = "925 Warden Ave, Scarborough, ON",
            ImageUrl = "/images/restaurants/karachi-bbq.jpg",
            DietaryTags = new List<string> { "Halal" }
        },
        new Restaurant
        {
            Id = 17,
            Name = "Plant Power",
            Cuisine = "Vegan",
            Location = "Toronto",
            Address = "410 Queen St W, Toronto, ON",
            ImageUrl = "/images/restaurants/plant-power.jpg",
            DietaryTags = new List<string> { "Vegan", "Gluten-Free" }
        },
        new Restaurant
        {
            Id = 18,
            Name = "La Crêperie",
            Cuisine = "French",
            Location = "Toronto",
            Address = "260 King St W, Toronto, ON",
            ImageUrl = "/images/restaurants/la-creperie.jpg",
            DietaryTags = new List<string> { "Vegetarian" }
        },
        new Restaurant
        {
            Id = 19,
            Name = "Caribbean Flavors",
            Cuisine = "Caribbean",
            Location = "Ajax",
            Address = "50 Bayly St W, Ajax, ON",
            ImageUrl = "/images/restaurants/caribbean-flavors.jpg",
            DietaryTags = new List<string> { "Gluten-Free" }
        },
        new Restaurant
        {
            Id = 20,
            Name = "Mama Africa",
            Cuisine = "African",
            Location = "Toronto",
            Address = "777 Dundas St E, Toronto, ON",
            ImageUrl = "/images/restaurants/mama-africa.jpg",
            DietaryTags = new List<string> { "Halal" }
        }
            };
        }
    }
}
