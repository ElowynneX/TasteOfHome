using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using TasteOfHome.Models;

namespace TasteOfHome.Services
{
    public class AiRecommendationService : IAiRecommendationService
    {
        private readonly HttpClient _httpClient;
        private readonly OpenAiOptions _options;

        public AiRecommendationService(HttpClient httpClient, IOptions<OpenAiOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task<AiRecommendationResult> GetRecommendationsAsync(string prompt, List<Restaurant> restaurants)
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
            });

            var dataJson = JsonSerializer.Serialize(simplified);

            var fullPrompt = $@"
You are a smart food recommendation assistant.

User request:
{prompt}

Restaurants:
{dataJson}

Rules:
- Pick ONLY 3 restaurants
- Use ONLY given data
- Give short reasons
- Do NOT invent prices
- Return JSON only

Format:
{{
  ""intro"": ""string"",
  ""suggestions"": [
    {{ ""restaurantId"": 0, ""name"": ""string"", ""reason"": ""string"" }}
  ]
}}
";

            var requestBody = new
            {
                model = _options.Model,
                input = fullPrompt
            };

            var request = new HttpRequestMessage(HttpMethod.Post, $"{_options.BaseUrl}/responses");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);

            request.Content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);

            var output = doc.RootElement
                .GetProperty("output")[0]
                .GetProperty("content")[0]
                .GetProperty("text")
                .GetString();

            return JsonSerializer.Deserialize<AiRecommendationResult>(output!,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }
    }
}