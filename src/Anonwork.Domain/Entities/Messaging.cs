using Anonwork.Domain.Common;

namespace Anonwork.Domain.Entities;

public class Conversation : AuditableEntity
{
    public ICollection<ConversationMember> Members { get; set; } = new List<ConversationMember>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}

public class ConversationMember
{
    public Guid ConversationId { get; set; }
    public Guid UserId { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastReadAt { get; set; }
    public Conversation Conversation { get; set; } = null!;
    public User User { get; set; } = null!;
}

public class Message : BaseEntity
{
    public Guid ConversationId { get; set; }
    public Guid SenderId { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsDeleted { get; set; } = false;
    public Conversation Conversation { get; set; } = null!;
    public User Sender { get; set; } = null!;
}
