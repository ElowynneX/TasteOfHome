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
            if (string.IsNullOrWhiteSpace(_options.ApiKey))
            {
                throw new InvalidOperationException("OpenAI ApiKey is missing.");
            }
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

            string outputText = "";

            if (doc.RootElement.TryGetProperty("output_text", out var outputProp))
            {
                outputText = outputProp.GetString() ?? "";
            }
            else if (doc.RootElement.TryGetProperty("output", out var outputArray))
            {
                // fallback parsing (new API format)
                var first = outputArray[0];
                var content = first.GetProperty("content")[0];
                outputText = content.GetProperty("text").GetString() ?? "";
            }

            if (string.IsNullOrWhiteSpace(outputText))
            {
                throw new InvalidOperationException("AI returned empty or unexpected response format.");
            }

            if (string.IsNullOrWhiteSpace(outputText))
            {
                throw new InvalidOperationException("OpenAI returned empty output_text.");
            }

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