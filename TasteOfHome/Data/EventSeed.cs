using TasteOfHome.Models;

namespace TasteOfHome.Data
{
    public static class EventSeed
    {
        public static List<CulturalEvent> GetEvents()
        {
            return new List<CulturalEvent>
            {
                new CulturalEvent
                {
                    Title = "TasteOfHome Diwali Food Festival",
                    Category = "Food Festival",
                    CultureTag = "Indian",
                    Description = "Celebrate Diwali with authentic sweets, street food, live cooking demos, and family-friendly performances hosted by TasteOfHome.",
                    City = "Toronto",
                    VenueName = "TasteOfHome Cultural Square",
                    Address = "100 Queen St W, Toronto, ON",
                    EventDate = DateTime.Today.AddDays(10),
                    StartTime = "6:00 PM",
                    EndTime = "10:00 PM",
                    Capacity = 80,
                    ReservedSpots = 32,
                    PricePerPerson = 15,
                    ImageUrl = "/images/events/diwali-food-festival.jpg",
                    IsFeatured = true,
                    IsActive = true
                },
                new CulturalEvent
                {
                    Title = "TasteOfHome Italian Pasta Workshop",
                    Category = "Cooking Workshop",
                    CultureTag = "Italian",
                    Description = "Learn handmade pasta, sauces, and plating with a chef-led workshop in a premium small-group setting.",
                    City = "Mississauga",
                    VenueName = "TasteOfHome Studio Kitchen",
                    Address = "215 Lakeshore Rd W, Mississauga, ON",
                    EventDate = DateTime.Today.AddDays(14),
                    StartTime = "2:00 PM",
                    EndTime = "5:00 PM",
                    Capacity = 20,
                    ReservedSpots = 14,
                    PricePerPerson = 39,
                    ImageUrl = "/images/events/italian-pasta-workshop.jpg",
                    IsFeatured = true,
                    IsActive = true
                },
                new CulturalEvent
                {
                    Title = "TasteOfHome Japanese Tea & Sushi Evening",
                    Category = "Community Gathering",
                    CultureTag = "Japanese",
                    Description = "An intimate evening with sushi tasting, tea pairing, and guided cultural dining etiquette.",
                    City = "Markham",
                    VenueName = "TasteOfHome Sakura Hall",
                    Address = "182 Main St Unionville, Markham, ON",
                    EventDate = DateTime.Today.AddDays(21),
                    StartTime = "6:30 PM",
                    EndTime = "9:00 PM",
                    Capacity = 30,
                    ReservedSpots = 11,
                    PricePerPerson = 28,
                    ImageUrl = "/images/events/japanese-tea-sushi-evening.jpg",
                    IsFeatured = true,
                    IsActive = true
                }
            };
        }
    }
}