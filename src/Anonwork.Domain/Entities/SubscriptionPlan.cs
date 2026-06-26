namespace Anonwork.Domain.Entities;

public partial class SubscriptionPlan
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? Description { get; set; }

    public long Price { get; set; }

    public int DurationDays { get; set; }

    public int MaxPostsPerDay { get; set; }

    public int MaxUploadsPerDay { get; set; }

    public int MaxPostFileSizeMb { get; set; }

    public int MaxPostImageCount { get; set; }

    public int MaxPostMediaCount { get; set; }

    public bool CanAttachMediaToPost { get; set; }

    public bool CanUploadPostFiles { get; set; }

    public bool CanUseExclusiveAnonImages { get; set; }

    public bool CanUsePremiumFeatures { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();
}
