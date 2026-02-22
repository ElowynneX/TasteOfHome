using TasteOfHome.Models;

namespace TasteOfHome.Data
{
    public class FeedbackSeed
    {
        public static List<Feedback> GetFeedbacks()
        {
            return new List<Feedback>
            {
                // =====================
                // Spice Garden (Id = 1)
                // =====================
                new Feedback(5, 95, "Deeply spiced curries that taste homemade.", 1),
                new Feedback(4, 88, "Excellent vegetarian dishes and fluffy naan.", 1),
                new Feedback(5, 92, "One of the most authentic Indian meals I've had in Toronto.", 1),
                new Feedback(5, 70, "Good flavors but slightly oily.", 1),
                new Feedback(5, 85, "Paneer dishes were rich and satisfying.", 1),
                new Feedback(5, 97, "Reminded me of food back home.", 1),
                new Feedback(5, 55, "Spice levels felt toned down.", 1),
                new Feedback(5, 83, "Great halal options and friendly service.", 1),
                new Feedback(5, 68, "Decent, but not memorable.", 1),
                new Feedback(5, 94, "Butter chicken was outstanding.", 1),
                new Feedback(5, 80, "Good value for the portions.", 1),
                new Feedback(5, 40, "Overcooked rice ruined the meal.", 1),
                new Feedback(5, 96, "Authentic flavors done right.", 1),
                new Feedback(4, 82, "Consistently good every visit.", 1),
                new Feedback(3, 72, "Solid choice, nothing special.", 1),
                new Feedback(5, 95, "Deeply spiced curries that taste homemade.", 1),
                new Feedback(4, 88, "Excellent vegetarian dishes and fluffy naan.", 1),
                new Feedback(5, 92, "One of the most authentic Indian meals I've had in Toronto.", 1),
                new Feedback(3, 70, "Good flavors but slightly oily.", 1),
                new Feedback(4, 85, "Paneer dishes were rich and satisfying.", 1),
                new Feedback(5, 97, "Reminded me of food back home.", 1),

                // =====================
                // Green Bowl (Id = 2)
                // =====================
                new Feedback(5, 90, "Fresh, vibrant vegan bowls with great texture.", 2),
                new Feedback(4, 85, "Loved the gluten-free options.", 2),
                new Feedback(5, 92, "Plant-based food done right.", 2),
                new Feedback(5, 70, "Healthy but a bit bland.", 2),
                new Feedback(5, 80, "Great sauces and variety.", 2),
                new Feedback(5, 60, "Portions felt small.", 2),
                new Feedback(5, 95, "Everything tasted fresh and intentional.", 2),
                new Feedback(4, 82, "Perfect lunch spot.", 2),
                new Feedback(3, 68, "Not very filling.", 2),
                new Feedback(5, 93, "Best vegan place in Waterloo.", 2),
                new Feedback(5, 78, "Good but slightly pricey.", 2),
                new Feedback(5, 45, "Too many raw components.", 2),
                new Feedback(5, 91, "Clean flavors and great balance.", 2),
                new Feedback(4, 84, "Gluten-free without compromise.", 2),
                new Feedback(5, 72, "Nice but forgettable.", 2),

                // =====================
                // Golden Wok (Id = 3)
                // =====================
                new Feedback(5, 85, "Classic Chinese flavors with a veggie focus.", 3),
                new Feedback(5, 92, "Very authentic stir-fry techniques.", 3),
                new Feedback(3, 70, "Sauces were a bit heavy.", 3),
                new Feedback(5, 80, "Vegetarian options are plentiful.", 3),
                new Feedback(5, 90, "Tastes like traditional home cooking.", 3),
                new Feedback(5, 58, "Noodles were overcooked.", 3),
                new Feedback(4, 83, "Good value and big portions.", 3),
                new Feedback(5, 65, "Average but reliable.", 3),
                new Feedback(5, 94, "One of Markham’s hidden gems.", 3),
                new Feedback(5, 78, "Friendly staff and fast service.", 3),
                new Feedback(5, 42, "Too greasy this time.", 3),
                new Feedback(5, 91, "Authentic wok flavor.", 3),
                new Feedback(4, 82, "Solid Chinese comfort food.", 3),
                new Feedback(3, 69, "Okay but not amazing.", 3),
                new Feedback(4, 86, "Vegetarian-friendly without compromise.", 3),
                new Feedback(5, 65, "Average but reliable.", 3),
                new Feedback(5, 94, "One of Markham’s hidden gems.", 3),
                new Feedback(4, 78, "Friendly staff and fast service.", 3),
                new Feedback(5, 42, "Too greasy this time.", 3),
                new Feedback(5, 91, "Authentic wok flavor.", 3),
                new Feedback(4, 82, "Solid Chinese comfort food.", 3),
                new Feedback(5, 69, "Okay but not amazing.", 3),
                new Feedback(4, 86, "Vegetarian-friendly without compromise.", 3),

                // =====================
                // Istanbul Grill (Id = 4)
                // =====================
                new Feedback(5, 93, "Perfectly seasoned meats and warm bread.", 4),
                new Feedback(4, 85, "Authentic Turkish flavors.", 4),
                new Feedback(5, 95, "Everything tasted incredibly fresh.", 4),
                new Feedback(5, 72, "Good but portions were smaller.", 4),
                new Feedback(5, 80, "Great halal option in Mississauga.", 4),
                new Feedback(5, 60, "Rice was a bit dry.", 4),
                new Feedback(5, 92, "Grilled meats were outstanding.", 4),
                new Feedback(4, 83, "Lovely spices and presentation.", 4),
                new Feedback(3, 68, "Decent but slow service.", 4),
                new Feedback(5, 94, "Tastes like food from Istanbul.", 4),
                new Feedback(4, 78, "Would definitely return.", 4),
                new Feedback(5, 48, "Overcooked kebab.", 4),
                new Feedback(5, 96, "Incredible authenticity.", 4),
                new Feedback(4, 82, "Consistently good quality.", 4),
                new Feedback(3, 70, "Solid meal overall.", 4),

                // =====================
                // Nonna’s Kitchen (Id = 5)
                // =====================
                new Feedback(5, 94, "Tastes like an Italian grandmother cooked it.", 5),
                new Feedback(4, 85, "Excellent vegetarian pasta options.", 5),
                new Feedback(5, 96, "Fresh ingredients and simple flavors.", 5),
                new Feedback(3, 72, "Sauce was slightly salty.", 5),
                new Feedback(4, 82, "Comforting and filling.", 5),
                new Feedback(5, 60, "Wait time was long.", 5),
                new Feedback(5, 93, "Authentic Italian comfort food.", 5),
                new Feedback(5, 80, "Cozy atmosphere and friendly staff.", 5),
                new Feedback(5, 70, "Good but not exceptional.", 5),
                new Feedback(5, 95, "Best vegetarian lasagna I’ve had.", 5),
                new Feedback(4, 78, "Portions are generous.", 5),
                new Feedback(1, 45, "Pasta was overcooked.", 5),
                new Feedback(5, 97, "True Italian soul food.", 5),
                new Feedback(4, 83, "Always reliable.", 5),
                new Feedback(3, 74, "Nice neighborhood spot.", 5),

                // =====================
                // Seoul Street (Id = 6)
                // =====================
                new Feedback(5, 90, "Bold flavors and excellent street-style dishes.", 6),
                new Feedback(4, 82, "Great balance of sweet and spicy.", 6),
                new Feedback(3, 70, "Good but lacked depth.", 6),
                new Feedback(5, 93, "Very authentic Korean taste.", 6),
                new Feedback(4, 80, "Loved the crispy textures.", 6),
                new Feedback(2, 60, "Too salty for my liking.", 6),
                new Feedback(5, 95, "Reminded me of Seoul night markets.", 6),
                new Feedback(4, 78, "Portions are generous.", 6),
                new Feedback(3, 68, "Decent but not memorable.", 6),
                new Feedback(5, 92, "Fantastic sauces.", 6),
                new Feedback(4, 84, "Quick service and tasty food.", 6),
                new Feedback(5, 45, "Food arrived cold.", 6),
                new Feedback(5, 96, "One of the best Korean spots nearby.", 6),
                new Feedback(4, 81, "Reliable choice.", 6),
                new Feedback(5, 72, "Okay overall.", 6),

                // =====================
                // Pho Saigon (Id = 7)
                // =====================
                new Feedback(5, 94, "Rich broth with deep flavor.", 7),
                new Feedback(5, 85, "Great halal pho option.", 7),
                new Feedback(5, 96, "Tastes just like Vietnam.", 7),
                new Feedback(3, 70, "Noodles were slightly mushy.", 7),
                new Feedback(4, 82, "Fresh herbs make a big difference.", 7),
                new Feedback(2, 60, "Portions were inconsistent.", 7),
                new Feedback(5, 93, "Excellent value for money.", 7),
                new Feedback(4, 80, "Comforting and filling.", 7),
                new Feedback(3, 68, "Broth could be hotter.", 7),
                new Feedback(5, 95, "Authentic and satisfying.", 7),
                new Feedback(4, 78, "Fast service.", 7),
                new Feedback(1, 48, "Too much oil on top.", 7),
                new Feedback(5, 97, "Best pho in Scarborough.", 7),
                new Feedback(4, 83, "Always consistent.", 7),
                new Feedback(3, 72, "Solid choice.", 7),

                // =====================
                // Tokyo Bento (Id = 8)
                // =====================
                new Feedback(4, 82, "Fresh bento boxes with good variety.", 8),
                new Feedback(5, 92, "Very authentic flavors.", 8),
                new Feedback(3, 70, "Rice was slightly dry.", 8),
                new Feedback(4, 80, "Great value lunch.", 8),
                new Feedback(5, 94, "Clean and well-balanced taste.", 8),
                new Feedback(2, 58, "Small portions.", 8),
                new Feedback(4, 83, "Nicely presented meals.", 8),
                new Feedback(5, 91, "Feels like a Tokyo takeaway.", 8),
                new Feedback(3, 68, "Average experience.", 8),
                new Feedback(4, 79, "Good consistency.", 8),
                new Feedback(5, 42, "Food arrived late.", 8),
                new Feedback(5, 95, "High-quality ingredients.", 8),
                new Feedback(4, 81, "Would return.", 8),
                new Feedback(3, 72, "Decent overall.", 8),
                new Feedback(5, 93, "One of my favorites downtown.", 8),

                // =====================
                // El Mariachi (Id = 9)
                // =====================
                new Feedback(5, 92, "Bold flavors and great seasoning.", 9),
                new Feedback(4, 84, "Loved the gluten-free options.", 9),
                new Feedback(3, 70, "Salsa lacked heat.", 9),
                new Feedback(5, 95, "Authentic Mexican taste.", 9),
                new Feedback(5, 80, "Great tacos.", 9),
                new Feedback(5, 60, "Dry tortillas.", 9),
                new Feedback(5, 93, "Fantastic street-style food.", 9),
                new Feedback(4, 82, "Friendly staff.", 9),
                new Feedback(3, 68, "Okay but not special.", 9),
                new Feedback(5, 94, "Really satisfying meal.", 9),
                new Feedback(4, 78, "Good portions.", 9),
                new Feedback(1, 45, "Food was cold.", 9),
                new Feedback(5, 96, "Closest thing to Mexico here.", 9),
                new Feedback(4, 83, "Always enjoyable.", 9),
                new Feedback(3, 72, "Decent option.", 9),

                // =====================
                // Falafel House (Id = 10)
                // =====================
                new Feedback(5, 95, "Crispy falafel and amazing sauces.", 10),
                new Feedback(4, 88, "Excellent vegan and halal options.", 10),
                new Feedback(5, 97, "Very authentic Middle Eastern flavors.", 10),
                new Feedback(3, 72, "Pita was a bit dry.", 10),
                new Feedback(4, 85, "Fresh ingredients.", 10),
                new Feedback(2, 60, "Slow service.", 10),
                new Feedback(5, 94, "Best falafel in Toronto.", 10),
                new Feedback(4, 82, "Great value.", 10),
                new Feedback(3, 70, "Decent but inconsistent.", 10),
                new Feedback(5, 96, "Perfect seasoning.", 10),
                new Feedback(4, 80, "Comfort food done right.", 10),
                new Feedback(5, 48, "Too greasy.", 10),
                new Feedback(5, 98, "Authenticity is off the charts.", 10),
                new Feedback(4, 84, "Reliable favorite.", 10),
                new Feedback(3, 74, "Nice casual spot.", 10),

                // =====================
                // Taste of Punjab (Id = 11)
                // =====================
                new Feedback(5, 96, "Rich and hearty Punjabi dishes.", 11),
                new Feedback(4, 85, "Excellent vegetarian options.", 11),
                new Feedback(5, 97, "Tastes just like home cooking.", 11),
                new Feedback(3, 72, "A bit heavy on oil.", 11),
                new Feedback(4, 82, "Very filling meals.", 11),
                new Feedback(2, 60, "Service was rushed.", 11),
                new Feedback(5, 94, "Authentic flavors throughout.", 11),
                new Feedback(4, 80, "Good value.", 11),
                new Feedback(3, 70, "Average visit.", 11),
                new Feedback(5, 95, "Comfort food at its best.", 11),
                new Feedback(4, 78, "Consistent quality.", 11),
                new Feedback(5, 45, "Too salty.", 11),
                new Feedback(5, 98, "True Punjabi taste.", 11),
                new Feedback(5, 83, "Always satisfying.", 11),
                new Feedback(5, 74, "Solid choice.", 11),

                // =====================
                // Bangkok Express (Id = 12)
                // =====================
                new Feedback(5, 93, "Perfect balance of sweet and spicy.", 12),
                new Feedback(4, 84, "Great gluten-free options.", 12),
                new Feedback(5, 95, "Authentic Thai flavors.", 12),
                new Feedback(3, 70, "Noodles were overcooked.", 12),
                new Feedback(4, 80, "Good lunch spot.", 12),
                new Feedback(5, 60, "Too mild for Thai food.", 12),
                new Feedback(5, 92, "Really satisfying curries.", 12),
                new Feedback(4, 82, "Fresh herbs stand out.", 12),
                new Feedback(3, 68, "Decent but nothing special.", 12),
                new Feedback(5, 94, "Consistently good.", 12),
                new Feedback(4, 78, "Quick service.", 12),
                new Feedback(5, 48, "Food arrived lukewarm.", 12),
                new Feedback(5, 96, "Feels authentic.", 12),
                new Feedback(4, 83, "Reliable spot.", 12),
                new Feedback(3, 72, "Okay overall.", 12),

                // =====================
                // Habesha Table (Id = 13)
                // =====================
                new Feedback(5, 97, "Incredible Ethiopian flavors.", 13),
                new Feedback(4, 88, "Fantastic vegan options.", 13),
                new Feedback(5, 98, "Very authentic and soulful food.", 13),
                new Feedback(3, 72, "Injera was a bit sour.", 13),
                new Feedback(4, 85, "Great shared platters.", 13),
                new Feedback(2, 60, "Service was slow.", 13),
                new Feedback(2, 96, "One of Toronto’s best.", 13),
                new Feedback(4, 82, "Rich spices.", 13),
                new Feedback(3, 70, "Good but heavy.", 13),
                new Feedback(5, 95, "Authenticity shines through.", 13),
                new Feedback(4, 80, "Generous portions.", 13),
                new Feedback(1, 48, "Food was cold.", 13),
                new Feedback(5, 99, "Exceptional experience.", 13),
                new Feedback(2, 84, "Highly recommend.", 13),
                new Feedback(2, 74, "Solid meal.", 13),

                // =====================
                // Casa Latina (Id = 14)
                // =====================
                new Feedback(5, 92, "Lively flavors and great variety.", 14),
                new Feedback(4, 82, "Enjoyed the Latin spices.", 14),
                new Feedback(3, 70, "Some dishes felt muted.", 14),
                new Feedback(5, 94, "Authentic taste overall.", 14),
                new Feedback(4, 80, "Fun atmosphere.", 14),
                new Feedback(2, 60, "Slow service.", 14),
                new Feedback(5, 93, "Great comfort food.", 14),
                new Feedback(4, 78, "Portions were generous.", 14),
                new Feedback(3, 68, "Average visit.", 14),
                new Feedback(5, 95, "Loved the seasoning.", 14),
                new Feedback(4, 83, "Would come back.", 14),
                new Feedback(1, 45, "Food lacked flavor.", 14),
                new Feedback(5, 96, "Authentic and hearty.", 14),
                new Feedback(4, 81, "Reliable spot.", 14),
                new Feedback(3, 72, "Okay overall.", 14),

                // =====================
                // Mediterraneo (Id = 15)
                // =====================
                new Feedback(5, 94, "Fresh Mediterranean flavors.", 15),
                new Feedback(4, 85, "Great vegetarian dishes.", 15),
                new Feedback(5, 96, "Very authentic Greek food.", 15),
                new Feedback(3, 72, "Portions were small.", 15),
                new Feedback(4, 80, "Well-seasoned dishes.", 15),
                new Feedback(2, 60, "Service was slow.", 15),
                new Feedback(5, 95, "Excellent olive oil and herbs.", 15),
                new Feedback(4, 82, "Lovely atmosphere.", 15),
                new Feedback(3, 70, "Decent but pricey.", 15),
                new Feedback(5, 97, "Best Greek food in Oakville.", 15),
                new Feedback(4, 78, "Consistent quality.", 15),
                new Feedback(1, 48, "Food arrived cold.", 15),
                new Feedback(5, 98, "Exceptional authenticity.", 15),
                new Feedback(4, 83, "Always good.", 15),
                new Feedback(3, 74, "Solid meal.", 15),

                // =====================
                // Karachi BBQ (Id = 16)
                // =====================
                new Feedback(5, 96, "Bold smoky flavors.", 16),
                new Feedback(4, 85, "Excellent halal BBQ.", 16),
                new Feedback(5, 97, "Authentic Pakistani taste.", 16),
                new Feedback(3, 72, "Meat was slightly dry.", 16),
                new Feedback(4, 82, "Great spice balance.", 16),
                new Feedback(2, 60, "Service was rushed.", 16),
                new Feedback(5, 95, "Perfectly grilled meats.", 16),
                new Feedback(4, 80, "Very filling.", 16),
                new Feedback(3, 70, "Average visit.", 16),
                new Feedback(5, 94, "Loved the kebabs.", 16),
                new Feedback(4, 78, "Good value.", 16),
                new Feedback(1, 45, "Too salty.", 16),
                new Feedback(5, 98, "True Karachi flavor.", 16),
                new Feedback(4, 83, "Always satisfying.", 16),
                new Feedback(3, 74, "Decent option.", 16),

                // =====================
                // Plant Power (Id = 17)
                // =====================
                new Feedback(5, 94, "Creative and filling vegan meals.", 17),
                new Feedback(4, 85, "Great gluten-free options.", 17),
                new Feedback(5, 96, "Very satisfying plant-based food.", 17),
                new Feedback(3, 72, "Some dishes were bland.", 17),
                new Feedback(4, 80, "Fresh ingredients.", 17),
                new Feedback(2, 60, "Overpriced.", 17),
                new Feedback(5, 95, "Best vegan spot downtown.", 17),
                new Feedback(4, 82, "Nice variety.", 17),
                new Feedback(3, 70, "Decent but inconsistent.", 17),
                new Feedback(5, 97, "Excellent flavors.", 17),
                new Feedback(4, 78, "Would return.", 17),
                new Feedback(1, 48, "Too much raw food.", 17),
                new Feedback(5, 98, "Very authentic vegan cooking.", 17),
                new Feedback(4, 83, "Reliable choice.", 17),
                new Feedback(3, 74, "Okay overall.", 17),

                // =====================
                // La Crêperie (Id = 18)
                // =====================
                new Feedback(5, 95, "Delicate and delicious crepes.", 18),
                new Feedback(4, 85, "Authentic French taste.", 18),
                new Feedback(5, 96, "Perfect texture every time.", 18),
                new Feedback(3, 72, "A bit sweet for my liking.", 18),
                new Feedback(4, 80, "Great vegetarian options.", 18),
                new Feedback(2, 60, "Slow service.", 18),
                new Feedback(5, 94, "Feels like Paris.", 18),
                new Feedback(4, 82, "Lovely café vibe.", 18),
                new Feedback(3, 70, "Decent visit.", 18),
                new Feedback(5, 97, "Exceptional quality.", 18),
                new Feedback(4, 78, "Good portions.", 18),
                new Feedback(1, 48, "Crepes were cold.", 18),
                new Feedback(5, 98, "True French authenticity.", 18),
                new Feedback(4, 83, "Always enjoyable.", 18),
                new Feedback(3, 74, "Solid meal.", 18),

                // =====================
                // Caribbean Flavors (Id = 19)
                // =====================
                new Feedback(5, 94, "Bold spices and rich sauces.", 19),
                new Feedback(4, 85, "Great gluten-free options.", 19),
                new Feedback(5, 96, "Very authentic Caribbean food.", 19),
                new Feedback(3, 72, "Too spicy for me.", 19),
                new Feedback(4, 80, "Good portion sizes.", 19),
                new Feedback(2, 60, "Slow service.", 19),
                new Feedback(5, 95, "Loved the jerk seasoning.", 19),
                new Feedback(4, 82, "Comfort food.", 19),
                new Feedback(3, 70, "Average experience.", 19),
                new Feedback(5, 97, "Full of flavor.", 19),
                new Feedback(4, 78, "Would recommend.", 19),
                new Feedback(1, 48, "Dry chicken.", 19),
                new Feedback(5, 98, "Authentic and delicious.", 19),
                new Feedback(4, 83, "Consistent quality.", 19),
                new Feedback(3, 74, "Okay overall.", 19),

                // =====================
                // Mama Africa (Id = 20)
                // =====================
                new Feedback(5, 97, "Rich and soulful African cuisine.", 20),
                new Feedback(4, 85, "Excellent halal dishes.", 20),
                new Feedback(5, 98, "Extremely authentic flavors.", 20),
                new Feedback(3, 72, "Heavy but satisfying.", 20),
                new Feedback(4, 82, "Great spice blends.", 20),
                new Feedback(2, 60, "Long wait time.", 20),
                new Feedback(5, 96, "One of a kind experience.", 20),
                new Feedback(4, 80, "Very filling meals.", 20),
                new Feedback(3, 70, "Decent but pricey.", 20),
                new Feedback(5, 99, "Top-tier authenticity.", 20),
                new Feedback(4, 78, "Would return.", 20),
                new Feedback(1, 48, "Food arrived cold.", 20),
                new Feedback(5, 98, "Exceptional cooking.", 20),
                new Feedback(4, 83, "Highly recommend.", 20),
                new Feedback(3, 74, "Solid meal.", 20)
            };
        }
    }
}
