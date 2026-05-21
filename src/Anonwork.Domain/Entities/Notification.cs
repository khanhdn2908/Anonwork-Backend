using System;
using System.Collections.Generic;

namespace Anonwork.Domain.Entities;

public partial class Notification
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid? ActorId { get; set; }

    public string Type { get; set; } = null!;

    public Guid? RefId { get; set; }

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User? Actor { get; set; }

    public virtual User User { get; set; } = null!;
}
