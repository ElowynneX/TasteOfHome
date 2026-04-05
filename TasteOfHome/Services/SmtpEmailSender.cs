using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TasteOfHome.Services;

public sealed class SmtpOptions
{
    public string Host { get; set; } = "";
    public int Port { get; set; } = 587;
    public string User { get; set; } = "";
    public string Pass { get; set; } = "";
    public string FromEmail { get; set; } = "";
    public string FromName { get; set; } = "TasteOfHome";
    public bool EnableSsl { get; set; } = true;
}

public sealed class SmtpEmailSender : IEmailSender
{
    private readonly SmtpOptions _opt;
    private readonly ILogger<SmtpEmailSender> _logger;

    public SmtpEmailSender(IOptions<SmtpOptions> opt, ILogger<SmtpEmailSender> logger)
    {
        _opt = opt.Value;
        _logger = logger;
    }

    public async Task SendAsync(string toEmail, string subject, string htmlBody)
    {
        if (string.IsNullOrWhiteSpace(_opt.Host))
            throw new InvalidOperationException("SMTP Host is not configured.");

        if (string.IsNullOrWhiteSpace(_opt.FromEmail))
            throw new InvalidOperationException("SMTP FromEmail is not configured.");

        if (string.IsNullOrWhiteSpace(toEmail))
            throw new InvalidOperationException("Recipient email is required.");

        try
        {
            using var msg = new MailMessage
            {
                From = new MailAddress(_opt.FromEmail, _opt.FromName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };

            msg.To.Add(new MailAddress(toEmail));

            using var client = new SmtpClient(_opt.Host, _opt.Port)
            {
                EnableSsl = _opt.EnableSsl,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_opt.User, _opt.Pass)
            };

            _logger.LogInformation("Sending email to {Email} with subject {Subject}", toEmail, subject);

            await client.SendMailAsync(msg);

            _logger.LogInformation("Email sent successfully to {Email}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SMTP email failed for {Email}", toEmail);
            throw;
        }
    }
}