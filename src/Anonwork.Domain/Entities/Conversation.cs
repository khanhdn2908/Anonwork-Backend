using System;
using System.Collections.Generic;

namespace Anonwork.Domain.Entities;

public partial class Conversation
{
    public Guid Id { get; set; }

    public bool IsGroup { get; set; }

    public string? Title { get; set; }

    public string? AvatarUrl { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<ConversationMember> ConversationMembers { get; set; } = new List<ConversationMember>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
