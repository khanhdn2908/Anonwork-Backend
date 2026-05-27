using Anonwork.Domain.Enums;
using System;
using System.Collections.Generic;

namespace Anonwork.Domain.Entities;

public partial class UserSubscription
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid PlanId { get; set; }

    public Guid OrderId { get; set; }

    public SubscriptionStatus Status { get; set; }

    public DateTime StartedAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual SubscriptionPlan Plan { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
