using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using TasteOfHome.Models;

namespace TasteOfHome.Services
{
    public class AiRestaurantEnrichmentService : IAiRestaurantEnrichmentService
    {
        private readonly HttpClient _httpClient;
        private readonly OpenAiOptions _options;

        public AiRestaurantEnrichmentService(
            HttpClient httpClient,
            IOptions<OpenAiOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task<RestaurantEnrichmentResult> EnrichRestaurantAsync(
            string name,
            string cuisine,
            string city,
            string address,
            string existingDietaryTagsCsv)
        {
            EnsureApiKey();

            var prompt = $$"""
You are helping enrich a multicultural restaurant discovery app called TasteOfHome.

Return ONLY valid JSON in this exact shape:
{
  "culturalStory": "string",
  "culturalTraditions": "string",
  "signatureDishes": ["string"],
  "dietaryTags": ["string"]
}

Restaurant details:
- Name: {{name}}
- Cuisine: {{cuisine}}
- City: {{city}}
- Address: {{address}}
- Existing dietary tags from source: {{existingDietaryTagsCsv}}

Rules:
- Keep it general and realistic.
- Do not invent owner history, awards, or exact business facts.
- Focus on the cuisine/cultural food tradition.
""";

            var requestBody = new
            {
                model = _options.Model,
                input = prompt,
                text = new
                {
                    format = new
                    {
                        type = "json_schema",
                        name = "restaurant_enrichment",
                        schema = new
                        {
                            type = "object",
                            additionalProperties = false,
                            properties = new
                            {
                                culturalStory = new { type = "string" },
                                culturalTraditions = new { type = "string" },
                                signatureDishes = new
                                {
                                    type = "array",
                                    items = new { type = "string" }
                                },
                                dietaryTags = new
                                {
                                    type = "array",
                                    items = new { type = "string" }
                                }
                            },
                            required = new[]
                            {
                                "culturalStory",
                                "culturalTraditions",
                                "signatureDishes",
                                "dietaryTags"
                            }
                        }
                    }
                }
            };

            var outputText = await SendResponsesRequestAsync(requestBody);

            var parsed = JsonSerializer.Deserialize<RestaurantEnrichmentResponse>(
                outputText,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (parsed == null)
            {
                throw new InvalidOperationException("Failed to parse AI enrichment JSON.");
            }

            return new RestaurantEnrichmentResult
            {
                CulturalStory = parsed.CulturalStory?.Trim() ?? "",
                CulturalTraditions = parsed.CulturalTraditions?.Trim() ?? "",
                SignatureDishes = CleanList(parsed.SignatureDishes, 5),
                DietaryTags = CleanList(parsed.DietaryTags, 5)
            };
        }

        public async Task<string> GetSearchSuggestionAsync(
            string searchQuery,
            string city,
            List<string> selectedCuisineFilters,
            List<string> selectedDietaryFilters)
        {
            EnsureApiKey();

            var cuisineText = selectedCuisineFilters != null && selectedCuisineFilters.Any()
                ? string.Join(", ", selectedCuisineFilters)
                : "None";

            var dietaryText = selectedDietaryFilters != null && selectedDietaryFilters.Any()
                ? string.Join(", ", selectedDietaryFilters)
                : "None";

            var prompt = $$"""
You are helping users search for restaurants in a multicultural restaurant discovery app called TasteOfHome.

User request:
- Search query: {{searchQuery}}
- City: {{(string.IsNullOrWhiteSpace(city) ? "Not specified" : city)}}
- Selected cuisine filters: {{cuisineText}}
- Selected dietary filters: {{dietaryText}}

Write a short, helpful suggestion in 1 or 2 sentences.
Rules:
- Keep it under 45 words.
- Be practical and user-friendly.
- Suggest what kind of restaurant or dishes the user may want.
- Do not use bullet points.
- Do not make up exact restaurant facts.
""";

            var requestBody = new
            {
                model = _options.Model,
                input = prompt
            };

            var outputText = await SendResponsesRequestAsync(requestBody);
            return outputText.Trim();
        }

        private async Task<string> SendResponsesRequestAsync(object requestBody)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"{_options.BaseUrl.TrimEnd('/')}/responses");

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", _options.ApiKey);

            request.Content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json");

            using var response = await _httpClient.SendAsync(request);
            var responseJson = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    $"OpenAI request failed. Status {(int)response.StatusCode}: {responseJson}");
            }

            using var doc = JsonDocument.Parse(responseJson);
            var outputText = ExtractOutputText(doc);

            if (string.IsNullOrWhiteSpace(outputText))
            {
                throw new InvalidOperationException("AI returned empty or unexpected response format.");
            }

            return outputText;
        }

        private static string ExtractOutputText(JsonDocument doc)
        {
            if (doc.RootElement.TryGetProperty("output_text", out var outputProp))
            {
                return outputProp.GetString() ?? "";
            }

            if (doc.RootElement.TryGetProperty("output", out var outputArray) &&
                outputArray.ValueKind == JsonValueKind.Array &&
                outputArray.GetArrayLength() > 0)
            {
                var first = outputArray[0];

                if (first.TryGetProperty("content", out var contentArray) &&
                    contentArray.ValueKind == JsonValueKind.Array &&
                    contentArray.GetArrayLength() > 0)
                {
                    var content = contentArray[0];

                    if (content.TryGetProperty("text", out var textProp))
                    {
                        return textProp.GetString() ?? "";
                    }
                }
            }

            return "";
        }

        private void EnsureApiKey()
        {
            if (string.IsNullOrWhiteSpace(_options.ApiKey))
            {
                throw new InvalidOperationException("OpenAI ApiKey is missing.");
            }
        }

        private static List<string> CleanList(List<string>? items, int max)
        {
            if (items == null)
                return new List<string>();

            return items
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Take(max)
                .ToList();
        }

        private sealed class RestaurantEnrichmentResponse
        {
            public string? CulturalStory { get; set; }
            public string? CulturalTraditions { get; set; }
            public List<string>? SignatureDishes { get; set; }
            public List<string>? DietaryTags { get; set; }
        }
    }
}