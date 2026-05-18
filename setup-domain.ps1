# Run script này từ thư mục gốc của solution (nơi có file Anonwork-Backend.sln)
# Chạy: .\setup-domain.ps1

$base = "src/Anonwork.Domain"

# Tạo thư mục
New-Item -ItemType Directory -Force -Path "$base/Common"
New-Item -ItemType Directory -Force -Path "$base/Enums"
New-Item -ItemType Directory -Force -Path "$base/Entities"

# ── BaseEntity.cs ──────────────────────────────────────────
@"
namespace Anonwork.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public abstract class AuditableEntity : BaseEntity
{
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
"@ | Set-Content "$base/Common/BaseEntity.cs"

# ── Enums.cs ───────────────────────────────────────────────
@"
namespace Anonwork.Domain.Enums;

public enum UserRole
{
    Student, Teacher, Moderator, Admin
}

public enum PostStatus
{
    Active, Pending, Removed
}

public enum VoteType
{
    Up, Down
}

public enum VoteTargetType
{
    Post, Comment
}

public enum NotificationType
{
    NewComment, Upvote, NewFollower, Mention, System, Ranking
}

public enum ReportTargetType
{
    Post, Comment
}

public enum ReportStatus
{
    Pending, Resolved, Dismissed
}

public enum SubscriptionPlan
{
    Free, PremiumMonth, PremiumYear
}

public enum PaymentStatus
{
    Pending, Success, Failed, Refunded
}
"@ | Set-Content "$base/Enums/Enums.cs"

# ── User.cs ────────────────────────────────────────────────
@"
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
    public UserRole Role { get; set; } = UserRole.Student;

    public ICollection<Post> Posts { get; set; } = new List<Post>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Vote> Votes { get; set; } = new List<Vote>();
    public ICollection<Bookmark> Bookmarks { get; set; } = new List<Bookmark>();
    public ICollection<Follow> Followers { get; set; } = new List<Follow>();
    public ICollection<Follow> Following { get; set; } = new List<Follow>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<Report> Reports { get; set; } = new List<Report>();
    public ICollection<ConversationMember> ConversationMembers { get; set; } = new List<ConversationMember>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}
"@ | Set-Content "$base/Entities/User.cs"

# ── Subject.cs ─────────────────────────────────────────────
@"
using Anonwork.Domain.Common;

namespace Anonwork.Domain.Entities;

public class Subject : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? IconEmoji { get; set; }
    public int PostCount { get; set; } = 0;

    public ICollection<Post> Posts { get; set; } = new List<Post>();
}
"@ | Set-Content "$base/Entities/Subject.cs"

# ── Post.cs ────────────────────────────────────────────────
@"
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
"@ | Set-Content "$base/Entities/Post.cs"

# ── Comment.cs ─────────────────────────────────────────────
@"
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
"@ | Set-Content "$base/Entities/Comment.cs"

# ── Interaction.cs ─────────────────────────────────────────
@"
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
"@ | Set-Content "$base/Entities/Interaction.cs"

# ── NotificationReport.cs ──────────────────────────────────
@"
using Anonwork.Domain.Common;
using Anonwork.Domain.Enums;

namespace Anonwork.Domain.Entities;

public class Notification : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid? ActorId { get; set; }
    public NotificationType Type { get; set; }
    public Guid? RefId { get; set; }
    public bool IsRead { get; set; } = false;
    public User User { get; set; } = null!;
    public User? Actor { get; set; }
}

public class Report : AuditableEntity
{
    public Guid ReporterId { get; set; }
    public Guid TargetId { get; set; }
    public ReportTargetType TargetType { get; set; }
    public string Reason { get; set; } = string.Empty;
    public ReportStatus Status { get; set; } = ReportStatus.Pending;
    public User Reporter { get; set; } = null!;
}
"@ | Set-Content "$base/Entities/NotificationReport.cs"

# ── Ranking.cs ─────────────────────────────────────────────
@"
using Anonwork.Domain.Common;

namespace Anonwork.Domain.Entities;

public class TrendingTag : BaseEntity
{
    public string Tag { get; set; } = string.Empty;
    public string Period { get; set; } = string.Empty;
    public int PostCount { get; set; } = 0;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class MonthlyRanking : BaseEntity
{
    public Guid PostId { get; set; }
    public string Period { get; set; } = string.Empty;
    public int RankPosition { get; set; }
    public int LikesCount { get; set; } = 0;
    public int CommentsCount { get; set; } = 0;
    public bool IsFinalized { get; set; } = false;
    public DateTime? FinalizedAt { get; set; }
    public Post Post { get; set; } = null!;
}
"@ | Set-Content "$base/Entities/Ranking.cs"

# ── Messaging.cs ───────────────────────────────────────────
@"
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
"@ | Set-Content "$base/Entities/Messaging.cs"

# ── Subscription.cs ────────────────────────────────────────
@"
using Anonwork.Domain.Common;
using Anonwork.Domain.Enums;

namespace Anonwork.Domain.Entities;

public class Subscription : BaseEntity
{
    public Guid UserId { get; set; }
    public SubscriptionPlan Plan { get; set; } = SubscriptionPlan.Free;
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; } = true;
    public User User { get; set; } = null!;
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}

public class Payment : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid SubscriptionId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "VND";
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public string? Provider { get; set; }
    public string? ProviderRef { get; set; }
    public User User { get; set; } = null!;
    public Subscription Subscription { get; set; } = null!;
}
"@ | Set-Content "$base/Entities/Subscription.cs"

Write-Host ""
Write-Host "✅ Domain entities created successfully!" -ForegroundColor Green
Write-Host ""
Write-Host "Files created:" -ForegroundColor Cyan
Get-ChildItem -Recurse "$base" -Filter "*.cs" | ForEach-Object { Write-Host "  $_" }