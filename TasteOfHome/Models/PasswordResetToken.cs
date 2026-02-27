using System.ComponentModel.DataAnnotations;

namespace TasteOfHome.Models;

public class PasswordResetToken
{
    public int Id { get; set; }

    public int UserId { get; set; }

    [Required]
    public string TokenHash { get; set; } = "";

    public DateTime ExpiresAtUtc { get; set; }
    public DateTime? UsedAtUtc { get; set; }
}