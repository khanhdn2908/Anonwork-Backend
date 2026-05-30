using Anonwork.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anonwork.Infrastructure.Persistence.Configuration;

public class PostTagConfiguration : IEntityTypeConfiguration<PostTag>
{
    public void Configure(EntityTypeBuilder<PostTag> entity)
    {
        entity.HasKey(e => new { e.PostId, e.Tag }).HasName("post_tags_pkey");
        entity.ToTable("post_tags");
        entity.HasIndex(e => e.Tag, "idx_post_tags_tag");
        entity.Property(e => e.PostId).HasColumnName("post_id");
        entity.Property(e => e.Tag).HasMaxLength(50).HasColumnName("tag");
        entity.HasOne(d => d.Post).WithMany(p => p.PostTags).HasForeignKey(d => d.PostId).HasConstraintName("post_tags_post_id_fkey");
    }
}