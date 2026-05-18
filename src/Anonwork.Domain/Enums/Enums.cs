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
