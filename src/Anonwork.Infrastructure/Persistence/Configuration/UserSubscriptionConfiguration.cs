using Anonwork.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anonwork.Infrastructure.Persistence.Configuration;

public class UserSubscriptionConfiguration : IEntityTypeConfiguration<UserSubscription>
{
    public void Configure(EntityTypeBuilder<UserSubscription> entity)
    {
        entity.HasKey(e => e.Id).HasName("user_subscriptions_pkey");
        entity.ToTable("user_subscriptions");
        entity.HasIndex(e => new { e.UserId, e.Status, e.ExpiresAt }, "idx_subscriptions_user");
        entity.HasIndex(e => new { e.UserId, e.PlanId }, "idx_one_active_sub").IsUnique().HasFilter("status = 0");
        entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()").HasColumnName("id");
        entity.Property(e => e.UserId).HasColumnName("user_id");
        entity.Property(e => e.PlanId).HasColumnName("plan_id");
        entity.Property(e => e.OrderId).HasColumnName("order_id");
        entity.Property(e => e.Status).HasMaxLength(20).HasColumnName("status");
        entity.Property(e => e.StartedAt).HasDefaultValueSql("now()").HasColumnName("started_at");
        entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");
        entity.HasOne(d => d.User).WithMany(p => p.UserSubscriptions).HasForeignKey(d => d.UserId).HasConstraintName("user_subscriptions_user_id_fkey");
        entity.HasOne(d => d.Plan).WithMany(p => p.UserSubscriptions).HasForeignKey(d => d.PlanId).HasConstraintName("user_subscriptions_plan_id_fkey");
        entity.HasOne(d => d.Order).WithMany(p => p.UserSubscriptions).HasForeignKey(d => d.OrderId).HasConstraintName("user_subscriptions_order_id_fkey");
    }
}