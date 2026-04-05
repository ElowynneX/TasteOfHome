using System.Globalization;
using System.Text.Json;
using TasteOfHome.Models;

namespace TasteOfHome.Services
{
    public class TicketmasterLiveEventsService : ILiveEventsService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public TicketmasterLiveEventsService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<List<LiveEvent>> SearchEventsAsync(
            string? keyword,
            string? city,
            string? category,
            double? latitude,
            double? longitude,
            int size = 20,
            CancellationToken cancellationToken = default)
        {
            var apiKey = _configuration["Ticketmaster:ApiKey"];
            var defaultCountryCode = _configuration["Ticketmaster:DefaultCountryCode"] ?? "CA";
            var defaultRadiusKm = _configuration["Ticketmaster:DefaultRadiusKm"] ?? "25";

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return new List<LiveEvent>();
            }

            var queryParts = new List<string>
            {
                $"apikey={Uri.EscapeDataString(apiKey)}",
                $"countryCode={Uri.EscapeDataString(defaultCountryCode)}",
                $"size={size}",
                "sort=date,asc",
                $"startDateTime={Uri.EscapeDataString(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"))}"
            };

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                queryParts.Add($"keyword={Uri.EscapeDataString(keyword.Trim())}");
            }

            if (!string.IsNullOrWhiteSpace(city))
            {
                queryParts.Add($"city={Uri.EscapeDataString(city.Trim())}");
            }

            if (latitude.HasValue && longitude.HasValue)
            {
                var latLong = $"{latitude.Value.ToString(CultureInfo.InvariantCulture)},{longitude.Value.ToString(CultureInfo.InvariantCulture)}";
                queryParts.Add($"latlong={Uri.EscapeDataString(latLong)}");
                queryParts.Add($"radius={Uri.EscapeDataString(defaultRadiusKm)}");
                queryParts.Add("unit=km");
            }

            var mappedClassification = MapCategory(category);
            if (!string.IsNullOrWhiteSpace(mappedClassification))
            {
                queryParts.Add($"classificationName={Uri.EscapeDataString(mappedClassification)}");
            }

            var url = "https://app.ticketmaster.com/discovery/v2/events.json?" + string.Join("&", queryParts);

            using var response = await _httpClient.GetAsync(url, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return new List<LiveEvent>();
            }

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

            var result = new List<LiveEvent>();

            if (!document.RootElement.TryGetProperty("_embedded", out var embedded))
                return result;

            if (!embedded.TryGetProperty("events", out var eventsArray))
                return result;

            foreach (var item in eventsArray.EnumerateArray())
            {
                var evt = new LiveEvent
                {
                    Id = GetString(item, "id"),
                    Title = GetString(item, "name"),
                    Description = GetString(item, "info"),
                    TicketUrl = GetString(item, "url"),
                    Status = GetNestedString(item, "dates", "status", "code"),
                    LocalDate = GetNestedString(item, "dates", "start", "localDate"),
                    LocalTime = GetNestedString(item, "dates", "start", "localTime"),
                    TimeZone = GetNestedString(item, "dates", "timezone"),
                    Category = GetPrimaryClassificationName(item),
                    CultureTag = InferCultureTag(item),
                    ImageUrl = GetBestImage(item),
                    PriceRangeText = GetPriceRange(item),
                    HasTickets = HasPublicSales(item)
                };

                var dateTimeRaw = GetNestedString(item, "dates", "start", "dateTime");
                if (DateTime.TryParse(dateTimeRaw, out var parsedUtc))
                {
                    evt.EventDateTimeUtc = parsedUtc;
                }

                if (item.TryGetProperty("_embedded", out var itemEmbedded) &&
                    itemEmbedded.TryGetProperty("venues", out var venues) &&
                    venues.ValueKind == JsonValueKind.Array &&
                    venues.GetArrayLength() > 0)
                {
                    var venue = venues[0];
                    evt.VenueName = GetString(venue, "name");
                    evt.City = GetNestedString(venue, "city", "name");
                    evt.Address = GetNestedString(venue, "address", "line1");
                }

                result.Add(evt);
            }

            return result;
        }

        private static string MapCategory(string? category)
        {
            if (string.IsNullOrWhiteSpace(category))
                return "";

            return category.Trim().ToLowerInvariant() switch
            {
                "concert" => "music",
                "music" => "music",
                "festival" => "miscellaneous",
                "food festival" => "miscellaneous",
                "workshop" => "arts & theatre",
                "cooking workshop" => "arts & theatre",
                "community gathering" => "family",
                "community" => "family",
                _ => category.Trim()
            };
        }

        private static string GetString(JsonElement element, string propertyName)
        {
            if (element.TryGetProperty(propertyName, out var value) && value.ValueKind == JsonValueKind.String)
            {
                return value.GetString() ?? "";
            }

            return "";
        }

        private static string GetNestedString(JsonElement element, params string[] path)
        {
            var current = element;

            foreach (var part in path)
            {
                if (!current.TryGetProperty(part, out var next))
                    return "";

                current = next;
            }

            return current.ValueKind == JsonValueKind.String ? current.GetString() ?? "" : "";
        }

        private static string GetPrimaryClassificationName(JsonElement item)
        {
            if (!item.TryGetProperty("classifications", out var classifications) ||
                classifications.ValueKind != JsonValueKind.Array)
            {
                return "";
            }

            foreach (var classification in classifications.EnumerateArray())
            {
                var segment = GetNestedString(classification, "segment", "name");
                var genre = GetNestedString(classification, "genre", "name");
                var subGenre = GetNestedString(classification, "subGenre", "name");

                if (!string.IsNullOrWhiteSpace(subGenre))
                    return subGenre;

                if (!string.IsNullOrWhiteSpace(genre))
                    return genre;

                if (!string.IsNullOrWhiteSpace(segment))
                    return segment;
            }

            return "";
        }

        private static string InferCultureTag(JsonElement item)
        {
            var text = (
                GetString(item, "name") + " " +
                GetString(item, "info") + " " +
                GetString(item, "pleaseNote")
            ).ToLowerInvariant();

            if (text.Contains("india") || text.Contains("indian") || text.Contains("diwali"))
                return "Indian";
            if (text.Contains("italy") || text.Contains("italian") || text.Contains("pasta"))
                return "Italian";
            if (text.Contains("japan") || text.Contains("japanese") || text.Contains("sushi"))
                return "Japanese";
            if (text.Contains("korea") || text.Contains("korean"))
                return "Korean";
            if (text.Contains("pakistan") || text.Contains("pakistani") || text.Contains("sufi"))
                return "Pakistani";
            if (text.Contains("caribbean"))
                return "Caribbean";
            if (text.Contains("mexican"))
                return "Mexican";
            if (text.Contains("chinese"))
                return "Chinese";

            return "Live Event";
        }

        private static string GetBestImage(JsonElement item)
        {
            if (!item.TryGetProperty("images", out var images) || images.ValueKind != JsonValueKind.Array)
                return "";

            string bestUrl = "";

            foreach (var image in images.EnumerateArray())
            {
                var url = GetString(image, "url");
                var ratio = GetString(image, "ratio");

                if (string.IsNullOrWhiteSpace(bestUrl))
                    bestUrl = url;

                if (ratio.Equals("16_9", StringComparison.OrdinalIgnoreCase))
                    return url;
            }

            return bestUrl;
        }

        private static string GetPriceRange(JsonElement item)
        {
            if (!item.TryGetProperty("priceRanges", out var prices) ||
                prices.ValueKind != JsonValueKind.Array ||
                prices.GetArrayLength() == 0)
            {
                return "See ticket site";
            }

            var first = prices[0];

            decimal? min = null;
            decimal? max = null;
            string currency = "";

            if (first.TryGetProperty("min", out var minValue) && minValue.ValueKind == JsonValueKind.Number)
            {
                min = minValue.GetDecimal();
            }

            if (first.TryGetProperty("max", out var maxValue) && maxValue.ValueKind == JsonValueKind.Number)
            {
                max = maxValue.GetDecimal();
            }

            currency = GetString(first, "currency");

            if (min.HasValue && max.HasValue)
            {
                return $"{currency} {min.Value:0.##} - {max.Value:0.##}";
            }

            if (min.HasValue)
            {
                return $"{currency} {min.Value:0.##}+";
            }

            return "See ticket site";
        }

        private static bool HasPublicSales(JsonElement item)
        {
            var start = GetNestedString(item, "sales", "public", "startDateTime");
            var end = GetNestedString(item, "sales", "public", "endDateTime");

            return !string.IsNullOrWhiteSpace(start) || !string.IsNullOrWhiteSpace(end);
        }
    }
}