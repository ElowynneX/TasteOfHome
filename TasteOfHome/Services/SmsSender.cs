using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TasteOfHome.Services
{
    public class SmsSender : ISmsSender
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SmsSender> _logger;

        public SmsSender(HttpClient httpClient, IConfiguration configuration, ILogger<SmsSender> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendSmsAsync(string toPhoneNumber, string message, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(toPhoneNumber))
                throw new ArgumentException("Recipient phone number is required.", nameof(toPhoneNumber));

            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("SMS message body is required.", nameof(message));

            var accountSid = _configuration["Twilio:AccountSid"];
            var authToken = _configuration["Twilio:AuthToken"];
            var fromNumber = _configuration["Twilio:FromNumber"];
            var messagingServiceSid = _configuration["Twilio:MessagingServiceSid"];

            if (string.IsNullOrWhiteSpace(accountSid) || string.IsNullOrWhiteSpace(authToken))
                throw new InvalidOperationException("Twilio config missing: AccountSid/AuthToken.");

            if (string.IsNullOrWhiteSpace(fromNumber) && string.IsNullOrWhiteSpace(messagingServiceSid))
                throw new InvalidOperationException("Twilio config missing: FromNumber or MessagingServiceSid.");

            var url = $"https://api.twilio.com/2010-04-01/Accounts/{accountSid}/Messages.json";

            using var request = new HttpRequestMessage(HttpMethod.Post, url);

            var authBytes = Encoding.ASCII.GetBytes($"{accountSid}:{authToken}");
            request.Headers.Authorization = new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(authBytes));

            var values = new Dictionary<string, string>
            {
                ["To"] = toPhoneNumber.Trim(),
                ["Body"] = message.Trim()
            };

            if (!string.IsNullOrWhiteSpace(messagingServiceSid))
                values["MessagingServiceSid"] = messagingServiceSid.Trim();
            else
                values["From"] = fromNumber!.Trim();

            request.Content = new FormUrlEncodedContent(values);

            _logger.LogInformation("Sending SMS to {Phone}", toPhoneNumber);

            using var response = await _httpClient.SendAsync(request, cancellationToken);
            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Twilio SMS failed. Status: {Status}. Response: {Body}", (int)response.StatusCode, body);
                throw new InvalidOperationException($"Twilio SMS failed: {body}");
            }

            _logger.LogInformation("SMS sent to {Phone}", toPhoneNumber);
        }
    }
}