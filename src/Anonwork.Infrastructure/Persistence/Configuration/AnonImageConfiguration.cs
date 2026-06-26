using Anonwork.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anonwork.Infrastructure.Persistence.Configuration;

public class AnonImageConfiguration : IEntityTypeConfiguration<AnonImage>
{
    public void Configure(EntityTypeBuilder<AnonImage> entity)
    {
        entity.HasKey(e => e.Id).HasName("anon_images_pkey");
        entity.ToTable("anon_images");

        entity.HasIndex(e => e.Name, "idx_anon_images_name");
        entity.HasIndex(e => e.IsActive, "idx_anon_images_is_active");
        entity.HasIndex(e => e.IsExclusive, "idx_anon_images_is_exclusive");

        entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()").HasColumnName("id");
        entity.Property(e => e.Name).HasMaxLength(120).HasColumnName("name");
        entity.Property(e => e.FileKey).HasColumnName("file_key");
        entity.Property(e => e.IsActive).HasDefaultValue(true).HasColumnName("is_active");
        entity.Property(e => e.IsExclusive).HasDefaultValue(false).HasColumnName("is_exclusive");
        entity.Property(e => e.ContentType).HasMaxLength(100).HasColumnName("content_type");
        entity.Property(e => e.FileSize).HasDefaultValue(0L).HasColumnName("file_size");
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");
        entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()").HasColumnName("updated_at");

        entity.HasMany(d => d.Users)
            .WithOne(p => p.AnonImage)
            .HasForeignKey(d => d.AnonImageId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("users_anon_image_id_fkey");
    }
}
