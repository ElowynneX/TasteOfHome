namespace TasteOfHome.Models
{
    public class Restaurant
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Cuisine { get; set; } = "";
        public string Location { get; set; } = "";
        public List<string> DietaryTags { get; set; } = new();
    }
}