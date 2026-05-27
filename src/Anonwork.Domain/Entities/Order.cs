using Anonwork.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Anonwork.Domain.Entities;

public partial class Order
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid? PlanId { get; set; }

    public string OrderCode { get; set; } = null!;

    public long Amount { get; set; }

    public string Currency { get; set; } = null!;

    public OrderStatus Status { get; set; }

    public PaymentMethod PaymentMethod { get; set; }

    public string? ProviderTransactionId { get; set; }

    public string? TransferContent { get; set; }

    public JsonDocument? Metadata { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public DateTime? PaidAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual SubscriptionPlan? Plan { get; set; }

    public virtual ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();
}
