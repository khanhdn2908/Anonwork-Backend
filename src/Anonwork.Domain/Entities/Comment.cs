using Anonwork.Domain.Common;

namespace Anonwork.Domain.Entities;

public class Comment : AuditableEntity
{
    public Guid PostId { get; set; }
    public Guid AuthorId { get; set; }
    public Guid? ParentId { get; set; }
    public bool IsAnonymous { get; set; } = false;
    public string Content { get; set; } = string.Empty;
    public int Upvotes { get; set; } = 0;

    public Post Post { get; set; } = null!;
    public User Author { get; set; } = null!;
    public Comment? Parent { get; set; }
    public ICollection<Comment> Replies { get; set; } = new List<Comment>();
}
