namespace Anonwork.Domain.Entities;

public partial class EmailVerificationToken
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string TokenHash { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public bool IsUsed { get; set; }
    public DateTime CreatedAt { get; set; }

    private EmailVerificationToken() { }

    public static EmailVerificationToken Create(string email, string username, string tokenHash, DateTime expiresAt)
        => new()
        {
            Id = Guid.NewGuid(),
            Email = email.ToLower().Trim(),
            Username = username.Trim(),
            TokenHash = tokenHash,
            ExpiresAt = expiresAt,
            IsUsed = false,
            CreatedAt = DateTime.UtcNow
        };

    public void MarkVerified()
    {
        IsUsed = true;
        VerifiedAt = DateTime.UtcNow;
    }
}