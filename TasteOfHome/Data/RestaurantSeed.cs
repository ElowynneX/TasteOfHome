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
                    DietaryTags = new List<string> { "Halal", "Vegetarian" },
                    CulturalStory = "A Punjabi family restaurant bringing traditional North Indian flavors to Toronto.",
                    CulturalTraditions = "Uses clay tandoor ovens and fresh ground spices daily.",
                    SignatureDishesCsv = "Butter Chicken,Dal Makhani,Tandoori Chicken,Garlic Naan"
                },

                new Restaurant
                {
                    Id = 2,
                    Name = "Green Bowl",
                    Cuisine = "Vegan",
                    Location = "Waterloo",
                    Address = "45 King St N, Waterloo, ON",
                    ImageUrl = "/images/restaurants/green-bowl.jpg",
                    DietaryTags = new List<string> { "Vegan", "Gluten-Free" },
                    CulturalStory = "Green Bowl celebrates plant-based cuisine inspired by global vegan traditions.",
                    CulturalTraditions = "Focus on sustainability and locally sourced ingredients.",
                    SignatureDishesCsv = "Vegan Buddha Bowl,Quinoa Power Salad,Avocado Wrap,Tofu Teriyaki Bowl"
                },

                new Restaurant
                {
                    Id = 3,
                    Name = "Golden Wok",
                    Cuisine = "Chinese",
                    Location = "Markham",
                    Address = "789 Highway 7, Markham, ON",
                    ImageUrl = "/images/restaurants/golden-wok.jpg",
                    DietaryTags = new List<string> { "Vegetarian" },
                    CulturalStory = "Golden Wok serves Cantonese-style family recipes passed down through generations.",
                    CulturalTraditions = "Traditional wok cooking techniques emphasizing fresh ingredients.",
                    SignatureDishesCsv = "Kung Pao Chicken,Sweet and Sour Pork,Vegetable Chow Mein,Dim Sum"
                },

                new Restaurant
                {
                    Id = 4,
                    Name = "Istanbul Grill",
                    Cuisine = "Turkish",
                    Location = "Mississauga",
                    Address = "310 Burnhamthorpe Rd, Mississauga, ON",
                    ImageUrl = "/images/restaurants/istanbul-grill.jpg",
                    DietaryTags = new List<string> { "Halal" },
                    CulturalStory = "Family-owned Turkish grill bringing authentic Anatolian cuisine.",
                    CulturalTraditions = "Charcoal grilled meats and traditional Turkish spices.",
                    SignatureDishesCsv = "Adana Kebab,Lamb Doner,Turkish Pide,Iskender Kebab"
                },

                new Restaurant
                {
                    Id = 5,
                    Name = "Nonna’s Kitchen",
                    Cuisine = "Italian",
                    Location = "Toronto",
                    Address = "88 College St, Toronto, ON",
                    ImageUrl = "/images/restaurants/nonna-kitchen.jpg",
                    DietaryTags = new List<string> { "Vegetarian" },
                    CulturalStory = "Inspired by Italian grandmothers' cooking traditions.",
                    CulturalTraditions = "Fresh pasta made daily using traditional Italian recipes.",
                    SignatureDishesCsv = "Spaghetti Carbonara,Margherita Pizza,Ravioli,Lasagna"
                },

                new Restaurant
                {
                    Id = 6,
                    Name = "Seoul Street",
                    Cuisine = "Korean",
                    Location = "North York",
                    Address = "512 Yonge St, North York, ON",
                    ImageUrl = "/images/restaurants/seoul-street.jpg",
                    DietaryTags = new List<string>(),
                    CulturalStory = "Street food flavors inspired by Seoul's famous night markets.",
                    CulturalTraditions = "Korean BBQ grilling and fermented dishes like kimchi.",
                    SignatureDishesCsv = "Bibimbap,Korean Fried Chicken,Bulgogi,Tteokbokki"
                },

                new Restaurant
                {
                    Id = 7,
                    Name = "Pho Saigon",
                    Cuisine = "Vietnamese",
                    Location = "Scarborough",
                    Address = "401 Lawrence Ave E, Scarborough, ON",
                    ImageUrl = "/images/restaurants/pho-saigon.jpg",
                    DietaryTags = new List<string> { "Halal", "Gluten-Free" },
                    CulturalStory = "Traditional Vietnamese noodle house inspired by Saigon street food.",
                    CulturalTraditions = "Slow simmered pho broth cooked for over 10 hours.",
                    SignatureDishesCsv = "Pho Beef Noodle Soup,Banh Mi,Summer Rolls,Vermicelli Bowl"
                },

                new Restaurant
                {
                    Id = 8,
                    Name = "Tokyo Bento",
                    Cuisine = "Japanese",
                    Location = "Toronto",
                    Address = "210 Dundas St W, Toronto, ON",
                    ImageUrl = "/images/restaurants/tokyo-bento.jpg",
                    DietaryTags = new List<string>(),
                    CulturalStory = "Japanese bento culture offering balanced meals.",
                    CulturalTraditions = "Precision cooking and seasonal ingredients.",
                    SignatureDishesCsv = "Sushi,Tempura,Ramen,Teriyaki Bento"
                },

                new Restaurant
                {
                    Id = 9,
                    Name = "El Mariachi",
                    Cuisine = "Mexican",
                    Location = "Brampton",
                    Address = "90 Main St S, Brampton, ON",
                    ImageUrl = "/images/restaurants/el-mariachi.jpg",
                    DietaryTags = new List<string> { "Gluten-Free" },
                    CulturalStory = "Colorful Mexican restaurant celebrating traditional street food.",
                    CulturalTraditions = "Hand-pressed tortillas and fresh salsa daily.",
                    SignatureDishesCsv = "Tacos Al Pastor,Chicken Quesadilla,Guacamole,Fajitas"
                },

                new Restaurant
                {
                    Id = 10,
                    Name = "Falafel House",
                    Cuisine = "Middle Eastern",
                    Location = "Toronto",
                    Address = "670 Bloor St W, Toronto, ON",
                    ImageUrl = "/images/restaurants/falafel-house.jpg",
                    DietaryTags = new List<string> { "Halal", "Vegetarian", "Vegan" },
                    CulturalStory = "A Middle Eastern eatery serving authentic Levantine dishes.",
                    CulturalTraditions = "Fresh falafel fried daily with tahini sauces.",
                    SignatureDishesCsv = "Falafel Wrap,Chicken Shawarma,Hummus Plate,Tabbouleh"
                },

                new Restaurant
                {
                    Id = 11,
                    Name = "Taste of Punjab",
                    Cuisine = "Indian",
                    Location = "Brampton",
                    Address = "188 Queen St E, Brampton, ON",
                    ImageUrl = "/images/restaurants/taste-of-punjab.jpg",
                    DietaryTags = new List<string> { "Vegetarian" },
                    CulturalStory = "Punjabi restaurant celebrating North Indian village cuisine.",
                    CulturalTraditions = "Traditional Punjabi tandoor and slow cooked curries.",
                    SignatureDishesCsv = "Chole Bhature,Paneer Tikka,Dal Tadka,Butter Chicken"
                },

                new Restaurant
                {
                    Id = 12,
                    Name = "Bangkok Express",
                    Cuisine = "Thai",
                    Location = "Toronto",
                    Address = "455 Spadina Ave, Toronto, ON",
                    ImageUrl = "/images/restaurants/bangkok-express.jpg",
                    DietaryTags = new List<string> { "Gluten-Free" },
                    CulturalStory = "Fast Thai street food inspired by Bangkok markets.",
                    CulturalTraditions = "Balance of sweet, sour, salty and spicy flavors.",
                    SignatureDishesCsv = "Pad Thai,Green Curry,Tom Yum Soup,Mango Sticky Rice"
                },

                new Restaurant
                {
                    Id = 13,
                    Name = "Habesha Table",
                    Cuisine = "Ethiopian",
                    Location = "Toronto",
                    Address = "337 Bloor St W, Toronto, ON",
                    ImageUrl = "/images/restaurants/habesha-table.jpg",
                    DietaryTags = new List<string> { "Vegetarian", "Vegan" },
                    CulturalStory = "Ethiopian communal dining experience served on injera bread.",
                    CulturalTraditions = "Shared platters reflecting Ethiopian hospitality.",
                    SignatureDishesCsv = "Doro Wat,Injera,Lentil Stew,Tibs"
                },

                new Restaurant
                {
                    Id = 14,
                    Name = "Casa Latina",
                    Cuisine = "Latin American",
                    Location = "Mississauga",
                    Address = "205 Lakeshore Rd E, Mississauga, ON",
                    ImageUrl = "/images/restaurants/casa-latina.jpg",
                    DietaryTags = new List<string>(),
                    CulturalStory = "Latin flavors representing multiple South American cultures.",
                    CulturalTraditions = "Family recipes with bold spices and grilled meats.",
                    SignatureDishesCsv = "Arepas,Empanadas,Grilled Steak,Plantains"
                },

                new Restaurant
                {
                    Id = 15,
                    Name = "Mediterraneo",
                    Cuisine = "Greek",
                    Location = "Oakville",
                    Address = "135 Kerr St, Oakville, ON",
                    ImageUrl = "/images/restaurants/mediterraneo.jpg",
                    DietaryTags = new List<string> { "Vegetarian" },
                    CulturalStory = "Greek coastal cuisine inspired by Mediterranean villages.",
                    CulturalTraditions = "Olive oil based dishes and charcoal grilled meats.",
                    SignatureDishesCsv = "Gyro,Souvlaki,Greek Salad,Moussaka"
                },

                new Restaurant
                {
                    Id = 16,
                    Name = "Karachi BBQ",
                    Cuisine = "Pakistani",
                    Location = "Scarborough",
                    Address = "925 Warden Ave, Scarborough, ON",
                    ImageUrl = "/images/restaurants/karachi-bbq.jpg",
                    DietaryTags = new List<string> { "Halal" },
                    CulturalStory = "Authentic Pakistani BBQ inspired by Karachi street food.",
                    CulturalTraditions = "Charcoal grilled kebabs and aromatic spices.",
                    SignatureDishesCsv = "Chicken Biryani,Seekh Kebab,Nihari,Chicken Karahi"
                },

                new Restaurant
                {
                    Id = 17,
                    Name = "Plant Power",
                    Cuisine = "Vegan",
                    Location = "Toronto",
                    Address = "410 Queen St W, Toronto, ON",
                    ImageUrl = "/images/restaurants/plant-power.jpg",
                    DietaryTags = new List<string> { "Vegan", "Gluten-Free" },
                    CulturalStory = "Modern vegan cafe promoting healthy plant-based eating.",
                    CulturalTraditions = "Focus on organic and nutrient-dense foods.",
                    SignatureDishesCsv = "Vegan Burger,Smoothie Bowl,Chickpea Wrap,Avocado Toast"
                },

                new Restaurant
                {
                    Id = 18,
                    Name = "La Crêperie",
                    Cuisine = "French",
                    Location = "Toronto",
                    Address = "260 King St W, Toronto, ON",
                    ImageUrl = "/images/restaurants/la-creperie.jpg",
                    DietaryTags = new List<string> { "Vegetarian" },
                    CulturalStory = "Parisian style crêperie bringing French street food culture.",
                    CulturalTraditions = "Thin crepes cooked on traditional flat griddles.",
                    SignatureDishesCsv = "Nutella Crepe,Ham and Cheese Crepe,French Onion Soup,Quiche"
                },

                new Restaurant
                {
                    Id = 19,
                    Name = "Caribbean Flavors",
                    Cuisine = "Caribbean",
                    Location = "Ajax",
                    Address = "50 Bayly St W, Ajax, ON",
                    ImageUrl = "/images/restaurants/caribbean-flavors.jpg",
                    DietaryTags = new List<string> { "Gluten-Free" },
                    CulturalStory = "Island cuisine inspired by Jamaican and Trinidadian flavors.",
                    CulturalTraditions = "Slow cooked jerk seasoning and tropical spices.",
                    SignatureDishesCsv = "Jerk Chicken,Rice and Peas,Curry Goat,Fried Plantains"
                },

                new Restaurant
                {
                    Id = 20,
                    Name = "Mama Africa",
                    Cuisine = "African",
                    Location = "Toronto",
                    Address = "777 Dundas St E, Toronto, ON",
                    ImageUrl = "/images/restaurants/mama-africa.jpg",
                    DietaryTags = new List<string> { "Halal" },
                    CulturalStory = "African cuisine celebrating rich regional traditions.",
                    CulturalTraditions = "Spice-rich stews and communal dining culture.",
                    SignatureDishesCsv = "Jollof Rice,Suya Grilled Meat,Pounded Yam,Efo Riro"
                }
            };
        }
    }
}