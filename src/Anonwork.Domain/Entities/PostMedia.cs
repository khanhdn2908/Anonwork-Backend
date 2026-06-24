using System;
using Anonwork.Domain.Enums;

namespace Anonwork.Domain.Entities;

public class PostMedia
{
    public Guid Id { get; set; }

    public Guid PostId { get; set; }

    public PostMediaType MediaType { get; set; }

    public string FileKey { get; set; } = null!;

    public string? ContentType { get; set; }

    public int DisplayOrder { get; set; }

    public long FileSize { get; set; }

    public string? OriginalFileName { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Post Post { get; set; } = null!;
}
