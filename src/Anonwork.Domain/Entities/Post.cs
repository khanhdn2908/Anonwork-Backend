using Anonwork.Domain.Common;
using Anonwork.Domain.Enums;

namespace Anonwork.Domain.Entities;

public class Post : AuditableEntity
{
    public Guid AuthorId { get; set; }
    public Guid SubjectId { get; set; }
    public bool IsAnonymous { get; set; } = false;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int Upvotes { get; set; } = 0;
    public int ViewCount { get; set; } = 0;
    public PostStatus Status { get; set; } = PostStatus.Active;

    public User Author { get; set; } = null!;
    public Subject Subject { get; set; } = null!;
    public ICollection<PostTag> Tags { get; set; } = new List<PostTag>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Bookmark> Bookmarks { get; set; } = new List<Bookmark>();
    public ICollection<MonthlyRanking> Rankings { get; set; } = new List<MonthlyRanking>();
}

public class PostTag
{
    public Guid PostId { get; set; }
    public string Tag { get; set; } = string.Empty;
    public Post Post { get; set; } = null!;
}
