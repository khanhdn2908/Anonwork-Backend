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
