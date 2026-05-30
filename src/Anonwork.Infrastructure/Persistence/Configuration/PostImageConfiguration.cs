using Anonwork.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anonwork.Infrastructure.Persistence.Configuration;

public class PostImageConfiguration : IEntityTypeConfiguration<PostImage>
{
    public void Configure(EntityTypeBuilder<PostImage> entity)
    {
        entity.HasKey(e => e.Id).HasName("post_images_pkey");
        entity.ToTable("post_images");
        entity.HasIndex(e => e.PostId, "idx_post_images_post");
        entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()").HasColumnName("id");
        entity.Property(e => e.PostId).HasColumnName("post_id");
        entity.Property(e => e.ImageUrl).HasColumnName("image_url");
        entity.Property(e => e.DisplayOrder).HasDefaultValue(0).HasColumnName("display_order");
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");
        entity.HasOne(d => d.Post).WithMany(p => p.PostImages).HasForeignKey(d => d.PostId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("post_images_post_id_fkey");
    }
}