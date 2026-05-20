using Anonwork.Domain.Common;
using Anonwork.Domain.Enums;

namespace Anonwork.Domain.Entities;

public class User : AuditableEntity
{
    public string Username { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string? Bio { get; set; }
    public string AnonAlias { get; set; } = string.Empty;
    public bool IsAnonDefault { get; set; } = false;
    public UserRole Role { get; set; } = UserRole.student;

    //public ICollection<Post> Posts { get; set; } = new List<Post>();
    //public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    //public ICollection<Vote> Votes { get; set; } = new List<Vote>();
    //public ICollection<Bookmark> Bookmarks { get; set; } = new List<Bookmark>();
    //public ICollection<Follow> Followers { get; set; } = new List<Follow>();
    //public ICollection<Follow> Followings { get; set; } = new List<Follow>();
    //public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    //public ICollection<Report> Reports { get; set; } = new List<Report>();
    //public ICollection<ConversationMember> ConversationMembers { get; set; } = new List<ConversationMember>();
    //public ICollection<Message> Messages { get; set; } = new List<Message>();
    //public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}
