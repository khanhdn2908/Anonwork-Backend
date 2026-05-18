using Anonwork.Domain.Common;
using Anonwork.Domain.Enums;

namespace Anonwork.Domain.Entities;

public class Vote : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid TargetId { get; set; }
    public VoteTargetType TargetType { get; set; }
    public VoteType VoteType { get; set; } = VoteType.Up;
    public User User { get; set; } = null!;
}

public class Follow : BaseEntity
{
    public Guid FollowerId { get; set; }
    public Guid FollowingId { get; set; }
    public User Follower { get; set; } = null!;
    public User FollowingUser { get; set; } = null!;
}

public class Bookmark : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid PostId { get; set; }
    public User User { get; set; } = null!;
    public Post Post { get; set; } = null!;
}
