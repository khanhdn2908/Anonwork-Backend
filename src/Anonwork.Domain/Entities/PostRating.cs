using System;

namespace Anonwork.Domain.Entities;

public partial class PostRating
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid PostId { get; set; }

    public Guid UserId { get; set; }

    public int Stars { get; set; }

    public string? Review { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public virtual Post Post { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
