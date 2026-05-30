using Anonwork.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anonwork.Infrastructure.Persistence.Configuration;

public class SubscriptionPlanConfiguration : IEntityTypeConfiguration<SubscriptionPlan>
{
    public void Configure(EntityTypeBuilder<SubscriptionPlan> entity)
    {
        entity.HasKey(e => e.Id).HasName("subscription_plans_pkey");
        entity.ToTable("subscription_plans");
        entity.HasIndex(e => e.Slug, "subscription_plans_slug_key").IsUnique();
        entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()").HasColumnName("id");
        entity.Property(e => e.Name).HasMaxLength(100).HasColumnName("name");
        entity.Property(e => e.Slug).HasMaxLength(50).HasColumnName("slug");
        entity.Property(e => e.Price).HasColumnName("price");
        entity.Property(e => e.DurationDays).HasColumnName("duration_days");
        entity.Property(e => e.Features).HasColumnType("jsonb").HasColumnName("features");
        entity.Property(e => e.IsActive).HasDefaultValue(true).HasColumnName("is_active");
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");
    }
}