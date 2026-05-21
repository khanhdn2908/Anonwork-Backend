using System;
using System.Collections.Generic;

namespace Anonwork.Domain.Entities;

public partial class ConversationMember
{
    public Guid ConversationId { get; set; }

    public Guid UserId { get; set; }

    public DateTime JoinedAt { get; set; }

    public DateTime? LastReadAt { get; set; }

    public virtual Conversation Conversation { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
