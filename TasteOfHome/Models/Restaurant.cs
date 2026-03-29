using System.ComponentModel.DataAnnotations.Schema;

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

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public string Source { get; set; } = "";
        public string ExternalId { get; set; } = "";
        public string PostalCode { get; set; } = "";
        public string City { get; set; } = "";

        public string DietaryTagsCsv { get; set; } = "";

        [NotMapped]
        public List<string> DietaryTags
        {
            get => DietaryTagsCsv
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(NormalizeDietaryTag)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            set => DietaryTagsCsv = string.Join(",",
                (value ?? new List<string>())
                    .Select(NormalizeDietaryTag)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Distinct(StringComparer.OrdinalIgnoreCase));
        }

        [NotMapped]
        public List<string> SignatureDishes
        {
            get => SignatureDishesCsv?
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList() ?? new List<string>();

            set => SignatureDishesCsv = string.Join(",",
                (value ?? new List<string>())
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Distinct(StringComparer.OrdinalIgnoreCase));
        }

        private static string NormalizeDietaryTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                return "";

            tag = tag.Trim();

            if (tag.Equals("vegetarian", StringComparison.OrdinalIgnoreCase))
                return "Vegetarian";

            if (tag.Equals("vegan", StringComparison.OrdinalIgnoreCase))
                return "Vegan";

            if (tag.Equals("halal", StringComparison.OrdinalIgnoreCase))
                return "Halal";

            if (tag.Equals("gluten free", StringComparison.OrdinalIgnoreCase) ||
                tag.Equals("gluten-free", StringComparison.OrdinalIgnoreCase))
                return "Gluten-Free";

            return tag;
        }
    }
}