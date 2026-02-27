using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TasteOfHome.Data;
using TasteOfHome.Models;

namespace TasteOfHome.Services;

public sealed class PasswordResetService
{
    private readonly AppDbContext _db;

    public PasswordResetService(AppDbContext db) => _db = db;

    public async Task<string?> CreateResetTokenAsync(string email)
    {
        var norm = (email ?? "").Trim().ToLowerInvariant();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == norm);
        if (user == null) return null;

        var raw = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
            .Replace("+", "-").Replace("/", "_").TrimEnd('=');

        var hash = Sha256Base64(raw);

        _db.PasswordResetTokens.Add(new PasswordResetToken
        {
            UserId = user.Id,
            TokenHash = hash,
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(30),
        });

        await _db.SaveChangesAsync();
        return raw;
    }

    public async Task<(bool ok, int userId)> ValidateTokenAsync(string email, string rawToken)
    {
        var norm = (email ?? "").Trim().ToLowerInvariant();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == norm);
        if (user == null) return (false, 0);

        var hash = Sha256Base64(rawToken);

        var record = await _db.PasswordResetTokens
            .Where(t => t.UserId == user.Id
                        && t.TokenHash == hash
                        && t.UsedAtUtc == null
                        && t.ExpiresAtUtc > DateTime.UtcNow)
            .OrderByDescending(t => t.Id)
            .FirstOrDefaultAsync();

        return record == null ? (false, 0) : (true, user.Id);
    }

    public async Task<bool> MarkUsedAsync(int userId, string rawToken)
    {
        var hash = Sha256Base64(rawToken);

        var record = await _db.PasswordResetTokens
            .Where(t => t.UserId == userId && t.TokenHash == hash && t.UsedAtUtc == null)
            .OrderByDescending(t => t.Id)
            .FirstOrDefaultAsync();

        if (record == null) return false;

        record.UsedAtUtc = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    private static string Sha256Base64(string input)
    {
        var bytes = Encoding.UTF8.GetBytes(input ?? "");
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }
}