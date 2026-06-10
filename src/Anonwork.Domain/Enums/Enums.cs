namespace Anonwork.Domain.Enums;

public enum PostStatus
{
    Pending,
    Published,
    Hidden,
    Deleted,
    Rejected
}

public enum ReportStatus
{
    Pending,
    Reviewing,
    Resolved,
    Rejected
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

public enum TokenPurpose
{
    EmailVerification,
    ForgotPassword,
    PasswordReset
}