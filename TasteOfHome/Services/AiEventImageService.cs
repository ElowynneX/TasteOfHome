using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using TasteOfHome.Models;

namespace TasteOfHome.Services
{
    public class AiEventImageService : IAiEventImageService
    {
        private readonly HttpClient _httpClient;
        private readonly OpenAiOptions _options;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<AiEventImageService> _logger;

        public AiEventImageService(
            HttpClient httpClient,
            IOptions<OpenAiOptions> options,
            IWebHostEnvironment environment,
            ILogger<AiEventImageService> logger)
        {
            _httpClient = httpClient;
            _options = options.Value;
            _environment = environment;
            _logger = logger;
        }

        public async Task<string?> GenerateEventImageAsync(
            EventImageGenerationRequest request,
            CancellationToken cancellationToken = default)
        {
            EnsureApiKey();

            var prompt = BuildPrompt(request);

            var requestBody = new
            {
                model = string.IsNullOrWhiteSpace(_options.ImageModel) ? "gpt-image-1" : _options.ImageModel,
                prompt,
                size = "1536x1024"
            };

            using var httpRequest = new HttpRequestMessage(
                HttpMethod.Post,
                $"{_options.BaseUrl.TrimEnd('/')}/images/generations");

            httpRequest.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _options.ApiKey);

            httpRequest.Content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json");

            using var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("OpenAI image generation failed. Status {StatusCode}. Response: {Response}",
                    (int)response.StatusCode, responseJson);
                return null;
            }

            using var doc = JsonDocument.Parse(responseJson);

            if (!doc.RootElement.TryGetProperty("data", out var dataArray) ||
                dataArray.ValueKind != JsonValueKind.Array ||
                dataArray.GetArrayLength() == 0)
            {
                _logger.LogWarning("OpenAI image generation returned no data.");
                return null;
            }

            var firstItem = dataArray[0];

            if (!firstItem.TryGetProperty("b64_json", out var b64Prop))
            {
                _logger.LogWarning("OpenAI image generation returned no b64_json.");
                return null;
            }

            var base64 = b64Prop.GetString();
            if (string.IsNullOrWhiteSpace(base64))
            {
                _logger.LogWarning("OpenAI image generation returned empty image data.");
                return null;
            }

            var bytes = Convert.FromBase64String(base64);
            var relativePath = await SaveImageAsync(bytes, request.Title, cancellationToken);

            return relativePath;
        }

        private async Task<string> SaveImageAsync(byte[] bytes, string title, CancellationToken cancellationToken)
        {
            var folder = Path.Combine(_environment.WebRootPath, "images", "events", "generated");
            Directory.CreateDirectory(folder);

            var safeTitle = ToSafeFileName(title);
            var fileName = $"{safeTitle}-{DateTime.UtcNow:yyyyMMddHHmmssfff}.png";
            var fullPath = Path.Combine(folder, fileName);

            await File.WriteAllBytesAsync(fullPath, bytes, cancellationToken);

            return $"/images/events/generated/{fileName}";
        }

        private static string BuildPrompt(EventImageGenerationRequest request)
        {
            var foodText = string.IsNullOrWhiteSpace(request.FoodDetails)
                ? "Culturally inspired food presentation"
                : request.FoodDetails.Trim();

            var entertainmentText = string.IsNullOrWhiteSpace(request.EntertainmentDetails)
                ? "Festive community atmosphere"
                : request.EntertainmentDetails.Trim();

            var dressText = string.IsNullOrWhiteSpace(request.DressCode)
                ? "Elegant casual cultural gathering"
                : request.DressCode.Trim();

            return $"""
Create a premium, realistic hero image for a cultural event listing in a food discovery app called TasteOfHome.

Event details:
- Title: {request.Title}
- Category: {request.Category}
- Culture: {request.CultureTag}
- Venue: {request.VenueName}
- City: {request.City}
- Description: {request.Description}
- Food details: {foodText}
- Entertainment details: {entertainmentText}
- Dress code style: {dressText}

Image requirements:
- realistic and premium
- vibrant cultural atmosphere
- cinematic lighting
- event-poster quality but without any text
- landscape orientation
- suitable for website hero/banner use
- visually rich, elegant, festive, welcoming
- include culturally relevant food, decor, and community vibe when appropriate
- no logos
- no watermark
- no typography
- no collage
""";
        }

        private void EnsureApiKey()
        {
            if (string.IsNullOrWhiteSpace(_options.ApiKey))
            {
                throw new InvalidOperationException("OpenAI API key is missing.");
            }
        }

        private static string ToSafeFileName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "event";

            var invalidChars = Path.GetInvalidFileNameChars();
            var cleaned = new string(value
                .Trim()
                .ToLowerInvariant()
                .Select(ch => invalidChars.Contains(ch) ? '-' : ch)
                .ToArray());

            cleaned = cleaned.Replace(' ', '-');

            while (cleaned.Contains("--"))
            {
                cleaned = cleaned.Replace("--", "-");
            }

            cleaned = cleaned.Trim('-');

            return string.IsNullOrWhiteSpace(cleaned) ? "event" : cleaned;
        }
    }
}