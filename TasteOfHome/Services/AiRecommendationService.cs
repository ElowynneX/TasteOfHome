using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using TasteOfHome.Models;

namespace TasteOfHome.Services
{
    public class AiRecommendationService : IAiRecommendationService
    {
        private readonly HttpClient _httpClient;
        private readonly OpenAiOptions _options;
        private readonly ILogger<AiRecommendationService> _logger;

        public AiRecommendationService(
            HttpClient httpClient,
            IOptions<OpenAiOptions> options,
            ILogger<AiRecommendationService> logger)
        {
            _httpClient = httpClient;
            _options = options.Value;
            _logger = logger;
        }

        public async Task<AiRecommendationResult> GetRecommendationsAsync(
            string prompt,
            List<Restaurant> restaurants)
        {
            var simplified = restaurants.Select(r => new
            {
                r.Id,
                r.Name,
                r.Cuisine,
                r.Location,
                r.Rating,
                r.Authenticity,
                r.DietaryTags
            }).ToList();

            var dataJson = JsonSerializer.Serialize(simplified);

            var fullPrompt =
                "You are a smart food recommendation assistant.\n\n" +
                $"User request:\n{prompt}\n\n" +
                $"Restaurants:\n{dataJson}\n\n" +
                "Rules:\n" +
                "- Pick ONLY 3 restaurants\n" +
                "- Use ONLY the given restaurant data\n" +
                "- Give short reasons\n" +
                "- Do NOT invent prices, distance, or details\n" +
                "- Return valid JSON only\n\n" +
                "Return exactly this shape:\n" +
                "{\n" +
                "  \"intro\": \"string\",\n" +
                "  \"suggestions\": [\n" +
                "    {\n" +
                "      \"restaurantId\": 0,\n" +
                "      \"name\": \"string\",\n" +
                "      \"reason\": \"string\"\n" +
                "    }\n" +
                "  ]\n" +
                "}";

            var requestBody = new
            {
                model = _options.Model,
                input = fullPrompt
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
            var rawJson = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("OpenAI status code: {StatusCode}", response.StatusCode);
            _logger.LogInformation("OpenAI raw response: {RawJson}", rawJson);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    "OpenAI request failed. Status: {StatusCode}, Body: {Body}",
                    response.StatusCode,
                    rawJson);

                return FallbackResult("AI service is unavailable right now.");
            }

            try
            {
                using var doc = JsonDocument.Parse(rawJson);
                var root = doc.RootElement;

                var text = ExtractResponseText(root);

                if (string.IsNullOrWhiteSpace(text))
                {
                    _logger.LogWarning("OpenAI response did not contain readable text output.");
                    return FallbackResult("No recommendations available right now.");
                }

                var result = JsonSerializer.Deserialize<AiRecommendationResult>(
                    text,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                if (result == null)
                {
                    _logger.LogWarning("AI returned invalid recommendation JSON: {Text}", text);
                    return FallbackResult("No recommendations available right now.");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to parse OpenAI recommendation response.");
                return FallbackResult("Unable to contact the recommendation service right now.");
            }
        }

        private static string? ExtractResponseText(JsonElement root)
        {
            if (root.TryGetProperty("output", out var outputElement) &&
                outputElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var outputItem in outputElement.EnumerateArray())
                {
                    if (outputItem.TryGetProperty("content", out var contentElement) &&
                        contentElement.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var contentItem in contentElement.EnumerateArray())
                        {
                            if (contentItem.TryGetProperty("text", out var textElement) &&
                                textElement.ValueKind == JsonValueKind.String)
                            {
                                return textElement.GetString();
                            }
                        }
                    }
                }
            }

            if (root.TryGetProperty("output_text", out var outputTextElement) &&
                outputTextElement.ValueKind == JsonValueKind.String)
            {
                return outputTextElement.GetString();
            }

            return null;
        }

        private static AiRecommendationResult FallbackResult(string intro)
        {
            return new AiRecommendationResult
            {
                Intro = intro
            };
        }
    }
}