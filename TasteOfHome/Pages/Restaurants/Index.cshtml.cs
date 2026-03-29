using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TasteOfHome.Data;
using TasteOfHome.Models;
using TasteOfHome.Services;

namespace TasteOfHome.Pages.Restaurants
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _db;
        private readonly IGooglePlacesService _googlePlacesService;

        private static readonly string[] SupportedDietaryTags =
        {
            "Vegetarian", "Vegan", "Halal", "Gluten-Free"
        };

        private static readonly string[] SupportedCuisineTags =
        {
            "Indian", "Chinese", "Middle Eastern", "Italian", "Vietnamese", "Ethiopian",
            "Mexican", "Thai", "Japanese", "Korean", "Mediterranean", "Pakistani"
        };

        private static readonly string[] KnownFoodKeywords =
        {
            "pizza", "biryani", "shawarma", "burger", "pasta", "sushi", "pho", "ramen",
            "falafel", "kebab", "taco", "tacos", "burrito", "noodles", "dumplings",
            "curry", "bbq", "barbecue", "wings", "sandwich", "salad", "steak",
            "seafood", "dessert", "coffee", "breakfast", "brunch", "chicken", "rice"
        };

        private static readonly string[] NonsenseKeywords =
        {
            "asdfgh", "qwerty", "zxcvbn", "poiuyt", "lkjhg", "mnbvc", "vdswfecewfc"
        };

        public IndexModel(AppDbContext db, IGooglePlacesService googlePlacesService)
        {
            _db = db;
            _googlePlacesService = googlePlacesService;
        }

        public List<Restaurant> Restaurants { get; set; } = new();
        public List<ImportedRestaurantDto> LiveRestaurants { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string SearchQuery { get; set; } = "";

        [BindProperty(SupportsGet = true)]
        public string City { get; set; } = "";

        [BindProperty(SupportsGet = true)]
        public List<string> SelectedDietaryFilters { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public List<string> SelectedCuisineFilter { get; set; } = new();

        public string SearchSummary { get; set; } = "";
        public string SearchError { get; set; } = "";

        public int TotalResultsCount => Restaurants.Count + LiveRestaurants.Count;

        public string GetImageFileName(int id)
        {
            return id switch
            {
                1 => "spice-garden.jpg",
                2 => "green-bowl.jpg",
                3 => "golden-wok.jpg",
                4 => "istanbul-grill.jpg",
                5 => "nonna-kitchen.jpg",
                6 => "seoul-street.jpg",
                7 => "pho-saigon.jpg",
                8 => "tokyo-bento.jpg",
                9 => "el-mariachi.jpg",
                10 => "falafel-house.jpg",
                11 => "taste-of-punjab.jpg",
                12 => "bangkok-express.jpg",
                13 => "habesha-table.jpg",
                14 => "casa-latina.jpg",
                15 => "mediterraneo.jpg",
                16 => "karachi-bbq.jpg",
                17 => "plant-power.jpg",
                18 => "la-creperie.jpg",
                19 => "caribbean-flavors.jpg",
                20 => "mama-africa.jpg",
                _ => "default.jpg"
            };
        }

        public string FormatCuisineLabel(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "Restaurant";

            var formatted = value.Replace("_", " ").Replace("-", " ");
            return string.Join(" ",
                formatted
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(word => char.ToUpperInvariant(word[0]) + word.Substring(1).ToLowerInvariant()));
        }

        public async Task OnGetAsync()
        {
            SearchQuery = NormalizeSearchText(SearchQuery?.Trim() ?? "");
            City = NormalizeCity(City?.Trim() ?? "");

            NormalizeSelectedFilters();
            ApplySmartSearchHints();

            if (string.IsNullOrWhiteSpace(City))
            {
                City = ExtractCityFromQuery(SearchQuery);
            }

            var cravingKeywords = ExtractCravingKeywords(SearchQuery, City)
                .Where(IsMeaningfulKeyword)
                .ToList();

            bool hasMeaningfulKeywords = cravingKeywords.Any();

            bool hasValidIntent =
                !string.IsNullOrWhiteSpace(City) ||
                SelectedCuisineFilter.Any() ||
                SelectedDietaryFilters.Any() ||
                hasMeaningfulKeywords;

            if (!hasValidIntent && !string.IsNullOrWhiteSpace(SearchQuery))
            {
                Restaurants = new List<Restaurant>();
                LiveRestaurants = new List<ImportedRestaurantDto>();
                SearchSummary = $"No restaurants found for \"{SearchQuery}\".";
                SearchError = "";
                return;
            }

            var dbQuery = _db.Restaurants
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(City))
            {
                var cityLower = City.ToLowerInvariant();

                dbQuery = dbQuery.Where(r =>
                    !string.IsNullOrWhiteSpace(r.Location) &&
                    r.Location.ToLower().Contains(cityLower));
            }

            var restaurants = await dbQuery
                .OrderByDescending(r => r.Rating)
                .ThenByDescending(r => r.Authenticity)
                .ToListAsync();

            restaurants = restaurants
                .Where(MatchesCuisineFilter)
                .Where(MatchesDietaryFilters)
                .ToList();

            if (cravingKeywords.Any())
            {
                restaurants = restaurants
                    .Where(r => MatchesRestaurantCraving(r, cravingKeywords))
                    .ToList();
            }

            Restaurants = restaurants;
            SearchSummary = BuildSearchSummary(cravingKeywords);

            if (hasValidIntent)
            {
                await LoadLiveNearbyRestaurantsAsync(cravingKeywords);
            }
        }

        private async Task LoadLiveNearbyRestaurantsAsync(List<string> cravingKeywords)
        {
            if (string.IsNullOrWhiteSpace(SearchQuery) &&
                string.IsNullOrWhiteSpace(City) &&
                !SelectedCuisineFilter.Any() &&
                !SelectedDietaryFilters.Any())
            {
                return;
            }

            try
            {
                var allLiveResults = new List<ImportedRestaurantDto>();
                var primaryQuery = BuildLiveSearchQuery(cravingKeywords);

                if (string.IsNullOrWhiteSpace(primaryQuery))
                {
                    LiveRestaurants = new List<ImportedRestaurantDto>();
                    return;
                }

                var primaryResults = await _googlePlacesService.SearchRestaurantsAsync(primaryQuery);
                allLiveResults.AddRange(primaryResults);

                if (!allLiveResults.Any() && !string.IsNullOrWhiteSpace(City))
                {
                    var fallbackQuery1 = $"restaurants in {City}";
                    var fallbackResults1 = await _googlePlacesService.SearchRestaurantsAsync(fallbackQuery1);
                    allLiveResults.AddRange(fallbackResults1);
                }

                if (!allLiveResults.Any() && !string.IsNullOrWhiteSpace(City))
                {
                    var fallbackQuery2 = $"food places near {City}";
                    var fallbackResults2 = await _googlePlacesService.SearchRestaurantsAsync(fallbackQuery2);
                    allLiveResults.AddRange(fallbackResults2);
                }

                var filteredLiveResults = allLiveResults
                    .Where(MatchesLiveCuisineFilter)
                    .Where(MatchesLiveDietaryFilters)
                    .Where(result => !Restaurants.Any(local =>
                        string.Equals(local.Name, result.Name, StringComparison.OrdinalIgnoreCase)))
                    .GroupBy(x => $"{x.Name}|{x.Address}")
                    .Select(g => g.First())
                    .Take(30)
                    .ToList();

                LiveRestaurants = filteredLiveResults;
            }
            catch
            {
                SearchError = "Live nearby results are unavailable right now. Your saved TasteOfHome results are still shown below.";
            }
        }

        private void NormalizeSelectedFilters()
        {
            SelectedDietaryFilters = SelectedDietaryFilters
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(NormalizeDietaryTag)
                .Where(x => SupportedDietaryTags.Contains(x, StringComparer.OrdinalIgnoreCase))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            SelectedCuisineFilter = SelectedCuisineFilter
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(NormalizeCuisineTag)
                .Where(x => SupportedCuisineTags.Contains(x, StringComparer.OrdinalIgnoreCase))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private void ApplySmartSearchHints()
        {
            foreach (var tag in SupportedDietaryTags)
            {
                if (SearchQuery.Contains(tag, StringComparison.OrdinalIgnoreCase))
                {
                    AddIfMissing(SelectedDietaryFilters, tag);
                }
            }

            foreach (var cuisine in SupportedCuisineTags)
            {
                if (SearchQuery.Contains(cuisine, StringComparison.OrdinalIgnoreCase))
                {
                    AddIfMissing(SelectedCuisineFilter, cuisine);
                }
            }

            if (SearchQuery.Contains("gluten free", StringComparison.OrdinalIgnoreCase) ||
                SearchQuery.Contains("gluten-free", StringComparison.OrdinalIgnoreCase))
            {
                AddIfMissing(SelectedDietaryFilters, "Gluten-Free");
            }
        }

        private bool MatchesCuisineFilter(Restaurant restaurant)
        {
            if (!SelectedCuisineFilter.Any())
                return true;

            if (string.IsNullOrWhiteSpace(restaurant.Cuisine))
                return false;

            return SelectedCuisineFilter.Any(filter =>
                string.Equals(restaurant.Cuisine, filter, StringComparison.OrdinalIgnoreCase));
        }

        private bool MatchesDietaryFilters(Restaurant restaurant)
        {
            if (!SelectedDietaryFilters.Any())
                return true;

            var tags = restaurant.DietaryTags ?? new List<string>();

            return SelectedDietaryFilters.All(filter =>
                tags.Any(tag => string.Equals(tag, filter, StringComparison.OrdinalIgnoreCase)));
        }

        private bool MatchesLiveCuisineFilter(ImportedRestaurantDto result)
        {
            if (!SelectedCuisineFilter.Any())
                return true;

            var searchableText = $"{result.Name} {result.Address}";

            return SelectedCuisineFilter.Any(filter =>
                searchableText.Contains(filter, StringComparison.OrdinalIgnoreCase));
        }

        private bool MatchesLiveDietaryFilters(ImportedRestaurantDto result)
        {
            if (!SelectedDietaryFilters.Any())
                return true;

            foreach (var filter in SelectedDietaryFilters)
            {
                if (filter.Equals("Vegetarian", StringComparison.OrdinalIgnoreCase) ||
                    filter.Equals("Vegan", StringComparison.OrdinalIgnoreCase))
                {
                    if (!result.ServesVegetarianFood)
                        return false;
                }
                else
                {
                    var searchableText = $"{result.Name} {result.Address}";
                    if (!searchableText.Contains(filter, StringComparison.OrdinalIgnoreCase))
                        return false;
                }
            }

            return true;
        }

        private List<string> ExtractCravingKeywords(string query, string city)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<string>();

            var stopWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "find", "show", "me", "best", "top", "good", "near", "nearby",
                "restaurant", "restaurants", "food", "place", "places",
                "in", "at", "around", "with", "for"
            };

            var reservedWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var filter in SelectedDietaryFilters)
            {
                foreach (var word in filter.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                    reservedWords.Add(word);
            }

            foreach (var filter in SelectedCuisineFilter)
            {
                foreach (var word in filter.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                    reservedWords.Add(word);
            }

            if (!string.IsNullOrWhiteSpace(city))
            {
                foreach (var word in city.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                    reservedWords.Add(word);
            }

            return query
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !stopWords.Contains(x))
                .Where(x => !reservedWords.Contains(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private bool MatchesRestaurantCraving(Restaurant restaurant, List<string> cravingKeywords)
        {
            if (!cravingKeywords.Any())
                return true;

            var searchableText = string.Join(" ",
                restaurant.Name ?? "",
                restaurant.Cuisine ?? "",
                restaurant.Location ?? "",
                restaurant.DietaryTags != null ? string.Join(" ", restaurant.DietaryTags) : "");

            return cravingKeywords.All(keyword =>
                searchableText.Contains(keyword, StringComparison.OrdinalIgnoreCase));
        }

        private bool IsMeaningfulKeyword(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return false;

            keyword = keyword.Trim().ToLowerInvariant();

            if (keyword.Length < 3)
                return false;

            if (NonsenseKeywords.Contains(keyword))
                return false;

            if (SupportedCuisineTags.Any(c => c.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
                return true;

            if (SupportedDietaryTags.Any(d => d.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
                return true;

            if (KnownFoodKeywords.Contains(keyword))
                return true;

            return false;
        }

        private string BuildLiveSearchQuery(List<string> cravingKeywords)
        {
            var parts = new List<string>();

            if (SelectedDietaryFilters.Any())
                parts.Add(string.Join(" ", SelectedDietaryFilters));

            if (SelectedCuisineFilter.Any())
                parts.Add(string.Join(" ", SelectedCuisineFilter));

            if (cravingKeywords.Any())
                parts.Add(string.Join(" ", cravingKeywords));

            string intentPart = parts.Any() ? string.Join(" ", parts) : "";

            if (!string.IsNullOrWhiteSpace(City) && !string.IsNullOrWhiteSpace(intentPart))
            {
                return $"{intentPart} near {City}".Trim();
            }

            if (!string.IsNullOrWhiteSpace(City))
            {
                return $"restaurants in {City}";
            }

            if (!string.IsNullOrWhiteSpace(intentPart))
            {
                return $"{intentPart} restaurants".Trim();
            }

            return "";
        }

        private string BuildSearchSummary(List<string> cravingKeywords)
        {
            var parts = new List<string>();

            if (SelectedDietaryFilters.Any())
                parts.Add(string.Join(", ", SelectedDietaryFilters));

            if (SelectedCuisineFilter.Any())
                parts.Add(string.Join(", ", SelectedCuisineFilter));

            if (cravingKeywords.Any())
                parts.Add(string.Join(", ", cravingKeywords));

            if (!string.IsNullOrWhiteSpace(City))
                parts.Add($"near {City}");

            if (!parts.Any())
                return "Browse curated cultural restaurants and live nearby food spots.";

            return $"Showing restaurants for {string.Join(" • ", parts)}.";
        }

        private static void AddIfMissing(List<string> items, string value)
        {
            if (!items.Any(x => x.Equals(value, StringComparison.OrdinalIgnoreCase)))
            {
                items.Add(value);
            }
        }

        private string NormalizeSearchText(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "";

            var normalized = input;

            var replacements = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["vegeterian"] = "vegetarian",
                ["vegitarian"] = "vegetarian",
                ["restuarant"] = "restaurant",
                ["restuarants"] = "restaurants",
                ["missisauga"] = "mississauga",
                ["gluten free"] = "gluten-free"
            };

            foreach (var item in replacements)
            {
                normalized = Regex.Replace(
                    normalized,
                    $@"\b{Regex.Escape(item.Key)}\b",
                    item.Value,
                    RegexOptions.IgnoreCase);
            }

            return normalized.Trim();
        }

        private string NormalizeDietaryTag(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "";

            input = input.Trim();

            if (input.Equals("gluten free", StringComparison.OrdinalIgnoreCase) ||
                input.Equals("gluten-free", StringComparison.OrdinalIgnoreCase))
                return "Gluten-Free";

            if (input.Equals("vegetarian", StringComparison.OrdinalIgnoreCase))
                return "Vegetarian";

            if (input.Equals("vegan", StringComparison.OrdinalIgnoreCase))
                return "Vegan";

            if (input.Equals("halal", StringComparison.OrdinalIgnoreCase))
                return "Halal";

            return input;
        }

        private string NormalizeCuisineTag(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "";

            return ToTitleCase(input.Trim());
        }

        private string NormalizeCity(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "";

            var city = input.Trim().ToLowerInvariant();

            if (city == "missisauga")
                city = "mississauga";

            return ToTitleCase(city);
        }

        private string ExtractCityFromQuery(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return "";

            var patterns = new[]
            {
                @"\b(?:restaurants?|food|places?)\s+in\s+(?<city>[a-zA-Z\s\-]+)$",
                @"\b(?:in|near|around)\s+(?<city>[a-zA-Z\s\-]+)$",
                @"^(?<city>[a-zA-Z\s\-]+)\s+restaurants?$"
            };

            foreach (var pattern in patterns)
            {
                var match = Regex.Match(query, pattern, RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    var city = match.Groups["city"].Value.Trim();
                    if (!string.IsNullOrWhiteSpace(city))
                        return NormalizeCity(city);
                }
            }

            return "";
        }

        private string ToTitleCase(string value)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.ToLowerInvariant());
        }
    }
}