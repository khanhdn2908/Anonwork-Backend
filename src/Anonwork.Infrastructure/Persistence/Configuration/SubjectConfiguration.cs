using Anonwork.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anonwork.Infrastructure.Persistence.Configuration;

public class SubjectConfiguration : IEntityTypeConfiguration<Subject>
{
    public void Configure(EntityTypeBuilder<Subject> entity)
    {
        entity.HasKey(e => e.Id).HasName("subjects_pkey");
        entity.ToTable("subjects");
        entity.HasIndex(e => e.Slug, "subjects_slug_key").IsUnique();
        entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()").HasColumnName("id");
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");
        entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()").HasColumnName("updated_at");
        entity.Property(e => e.IsActive).HasDefaultValue(true).HasColumnName("is_active");
        entity.Property(e => e.IconEmoji).HasMaxLength(10).HasColumnName("icon_emoji");
        entity.Property(e => e.Name).HasMaxLength(100).HasColumnName("name");
        entity.Property(e => e.PostCount).HasDefaultValue(0).HasColumnName("post_count");
        entity.Property(e => e.Slug).HasMaxLength(100).HasColumnName("slug");
    }
}