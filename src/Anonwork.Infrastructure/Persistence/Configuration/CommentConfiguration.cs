using Anonwork.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anonwork.Infrastructure.Persistence.Configuration;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> entity)
    {
        entity.HasKey(e => e.Id).HasName("comments_pkey");
        entity.ToTable("comments");
        entity.HasIndex(e => e.ParentId, "idx_comments_parent");
        entity.HasIndex(e => e.PostId, "idx_comments_post");
        entity.HasIndex(e => new { e.PostId, e.CreatedAt }, "idx_comments_post_created");
        entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()").HasColumnName("id");
        entity.Property(e => e.AuthorId).HasColumnName("author_id");
        entity.Property(e => e.Content).HasColumnName("content");
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");
        entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
        entity.Property(e => e.Depth).HasDefaultValue(0).HasColumnName("depth");
        entity.Property(e => e.IsAnonymous).HasDefaultValue(false).HasColumnName("is_anonymous");
        entity.Property(e => e.IsDeleted).HasDefaultValue(false).HasColumnName("is_deleted");
        entity.Property(e => e.ParentId).HasColumnName("parent_id");
        entity.Property(e => e.PostId).HasColumnName("post_id");
        entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()").HasColumnName("updated_at");
        entity.Property(e => e.Upvotes).HasDefaultValue(0).HasColumnName("upvotes");
        entity.HasOne(d => d.Author).WithMany(p => p.Comments).HasForeignKey(d => d.AuthorId).HasConstraintName("comments_author_id_fkey");
        entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent).HasForeignKey(d => d.ParentId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("comments_parent_id_fkey");
        entity.HasOne(d => d.Post).WithMany(p => p.Comments).HasForeignKey(d => d.PostId).HasConstraintName("comments_post_id_fkey");
    }
}