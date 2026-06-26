using System;
using System.Collections.Generic;

namespace Anonwork.Domain.Entities;

public class AnonImage
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string FileKey { get; set; } = null!;

    public bool IsActive { get; set; } = true;

    public bool IsExclusive { get; set; }

    public string? ContentType { get; set; }

    public long FileSize { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
