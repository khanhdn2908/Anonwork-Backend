using System;
using System.Collections.Generic;

namespace Anonwork.Domain.Entities;

public partial class Bookmark
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid PostId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Post Post { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
