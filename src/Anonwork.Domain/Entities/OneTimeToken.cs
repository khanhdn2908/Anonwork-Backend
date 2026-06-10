using Anonwork.Domain.Enums;

namespace Anonwork.Domain.Entities;

public partial class OneTimeToken
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string? Username { get; set; }
    public TokenPurpose Purpose { get; set; }
    public string TokenHash { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public DateTime? UsedAt { get; set; }
    public DateTime CreatedAt { get; set; }

    private OneTimeToken() { }

    public static OneTimeToken Create(
        string email,
        string tokenHash,
        TokenPurpose purpose,
        DateTime expiresAt,
        string? username = null)
        => new()
        {
            Id = Guid.NewGuid(),
            Email = email.ToLower().Trim(),
            Username = string.IsNullOrWhiteSpace(username) ? null : username.Trim(),
            Purpose = purpose,
            TokenHash = tokenHash,
            ExpiresAt = expiresAt,
            CreatedAt = DateTime.UtcNow
        };

    public bool IsExpired => DateTime.UtcNow > ExpiresAt;
    public bool IsUsed => UsedAt is not null;

    public void MarkUsed()
    {
        UsedAt = DateTime.UtcNow;
    }
}
