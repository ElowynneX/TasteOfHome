using System.Net;
using System.Net.Mail;
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

    public SmtpEmailSender(IOptions<SmtpOptions> opt) => _opt = opt.Value;

    public async Task SendAsync(string toEmail, string subject, string htmlBody)
    {
        if (string.IsNullOrWhiteSpace(_opt.Host))
            throw new InvalidOperationException("SMTP Host is not configured.");

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
            Credentials = string.IsNullOrWhiteSpace(_opt.User)
                ? CredentialCache.DefaultNetworkCredentials
                : new NetworkCredential(_opt.User, _opt.Pass),
        };

        await Task.Run(() => client.Send(msg));
    }
}