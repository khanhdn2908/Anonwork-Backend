namespace Anonwork.Domain.Entities;

/// <summary>
/// Entity representing audit/activity logs of user actions.
/// </summary>
public partial class UserActivityLog
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid? UserId { get; set; }

    public string Action { get; set; } = null!;

    public string ActionCategory { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string? TargetType { get; set; }

    public string? TargetId { get; set; }

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public string? DetailsJson { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual User? User { get; set; }
}
