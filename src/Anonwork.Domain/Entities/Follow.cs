using System;
using System.Collections.Generic;

namespace Anonwork.Domain.Entities;

public partial class Follow
{
    public Guid Id { get; set; }

    public Guid FollowerId { get; set; }

    public Guid FollowingId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User Follower { get; set; } = null!;

    public virtual User Following { get; set; } = null!;
}
