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
        entity.Property(e => e.Name).HasMaxLength(100).IsRequired().HasColumnName("name");
        entity.Property(e => e.Slug).HasMaxLength(100).IsRequired().HasColumnName("slug");
        entity.Property(e => e.Description).HasMaxLength(1000).HasColumnName("description");
        entity.Property(e => e.Price).HasColumnName("price");
        entity.Property(e => e.DurationDays).HasColumnName("duration_days");
        entity.Property(e => e.MaxPostsPerDay).HasDefaultValue(0).HasColumnName("max_posts_per_day");
        entity.Property(e => e.MaxUploadsPerDay).HasDefaultValue(0).HasColumnName("max_uploads_per_day");
        entity.Property(e => e.MaxPostFileSizeMb).HasDefaultValue(0).HasColumnName("max_post_file_size_mb");
        entity.Property(e => e.MaxPostImageCount).HasDefaultValue(0).HasColumnName("max_post_image_count");
        entity.Property(e => e.MaxPostMediaCount).HasDefaultValue(0).HasColumnName("max_post_media_count");
        entity.Property(e => e.CanAttachMediaToPost).HasDefaultValue(false).HasColumnName("can_attach_media_to_post");
        entity.Property(e => e.CanUploadPostFiles).HasDefaultValue(false).HasColumnName("can_upload_post_files");
        entity.Property(e => e.CanUseExclusiveAnonImages).HasDefaultValue(false).HasColumnName("can_use_exclusive_anon_images");
        entity.Property(e => e.CanUsePremiumFeatures).HasDefaultValue(false).HasColumnName("can_use_premium_features");
        entity.Property(e => e.IsActive).HasDefaultValue(true).HasColumnName("is_active");
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");
        entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
    }
}
