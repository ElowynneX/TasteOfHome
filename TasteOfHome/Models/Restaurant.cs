namespace TasteOfHome.Models
{
    public class Restaurant
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Cuisine { get; set; } = "";
        public string Location { get; set; } = "";
        public string Address { get; set; } = "";
        public int Authenticity { get; set; } = 0;
        public float Rating { get; set; } = 0;
        public int NumberOfReviews { get; set; } = 0;

        public string? ImageUrl { get; set; }

        public string? CulturalStory { get; set; }
        public string? CulturalTraditions { get; set; }
        public string? SignatureDishesCsv { get; set; }


        public string DietaryTagsCsv { get; set; } = "";

       
        public List<string> DietaryTags
        {
            get => DietaryTagsCsv
                .Split(',', System.StringSplitOptions.RemoveEmptyEntries | System.StringSplitOptions.TrimEntries)
                .ToList();

            set => DietaryTagsCsv = string.Join(',', value ?? new List<string>());
        }

        public List<string> SignatureDishes
        {
            get => SignatureDishesCsv?
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList() ?? new List<string>();

            set => SignatureDishesCsv = string.Join(',', value ?? new List<string>());
        }
    }
}
