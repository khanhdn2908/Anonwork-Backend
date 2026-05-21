using System;
using System.Collections.Generic;

namespace Anonwork.Domain.Entities;

public partial class Vote
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid TargetId { get; set; }

    public string TargetType { get; set; } = null!;

    public string VoteType { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
