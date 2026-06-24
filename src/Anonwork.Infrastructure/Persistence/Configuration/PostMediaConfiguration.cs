using Anonwork.Domain.Entities;
using Anonwork.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anonwork.Infrastructure.Persistence.Configuration;

public class PostMediaConfiguration : IEntityTypeConfiguration<PostMedia>
{
    public void Configure(EntityTypeBuilder<PostMedia> entity)
    {
        entity.HasKey(e => e.Id).HasName("post_media_pkey");
        entity.ToTable("post_media");
        entity.HasIndex(e => e.PostId, "idx_post_media_post");
        entity.HasIndex(e => new { e.PostId, e.DisplayOrder }, "idx_post_media_post_order");

        entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()").HasColumnName("id");
        entity.Property(e => e.PostId).HasColumnName("post_id");
        entity.Property(e => e.MediaType).HasConversion<string>().HasMaxLength(20).HasColumnName("media_type");
        entity.Property(e => e.FileKey).HasMaxLength(500).HasColumnName("file_key");
        entity.Property(e => e.ContentType).HasMaxLength(120).HasColumnName("content_type");
        entity.Property(e => e.DisplayOrder).HasDefaultValue(0).HasColumnName("display_order");
        entity.Property(e => e.FileSize).HasDefaultValue(0).HasColumnName("file_size");
        entity.Property(e => e.OriginalFileName).HasMaxLength(255).HasColumnName("original_file_name");
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");

        entity.HasOne(d => d.Post)
            .WithMany(p => p.PostMediaItems)
            .HasForeignKey(d => d.PostId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("post_media_post_id_fkey");
    }
}
