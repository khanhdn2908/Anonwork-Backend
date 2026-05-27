namespace Anonwork.Domain.Enums;

public enum UserRole
{
    student, teacher, moderator, admin
}

public enum PostStatus
{
    active, pending, removed
}

public enum VoteType
{
    up, down
}

public enum VoteTargetType
{
    post, comment
}

public enum NotificationType
{
    newComment, upvote, newfollower, mention, system, ranking
}

public enum ReportTargetType
{
    post, comment
}

public enum ReportStatus
{
    pending, resolved, dismissed
}

public enum SubscriptionPlan
{
    free, premiummonth, premiumyear
}

public enum PaymentStatus
{
    pending, success, failed, refunded
}

public enum OrderStatus
{
    Pending,
    Paid,
    Failed,
    Refunded,
    Expired
}

public enum PaymentMethod
{
    BankTransfer,
    Momo,
    VNPay,
    Stripe
}

public enum SubscriptionStatus
{
    Active,
    Expired,
    Cancelled
}