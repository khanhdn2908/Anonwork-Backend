using Anonwork.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anonwork.Infrastructure.Persistence.Configuration;

public class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> entity)
    {
        entity.HasKey(e => e.Id).HasName("posts_pkey");
        entity.ToTable("posts");
        entity.HasIndex(e => e.AuthorId, "idx_posts_author");
        entity.HasIndex(e => new { e.Status, e.CreatedAt }, "idx_posts_feed").IsDescending(false, true);
        entity.HasIndex(e => e.SearchVector, "idx_posts_search").HasMethod("gin");
        entity.HasIndex(e => new { e.SubjectId, e.CreatedAt }, "idx_posts_subject").IsDescending(false, true);
        entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()").HasColumnName("id");
        entity.Property(e => e.AuthorId).HasColumnName("author_id");
        entity.Property(e => e.CommentsCount).HasDefaultValue(0).HasColumnName("comments_count");
        entity.Property(e => e.Content).HasColumnName("content");
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");
        entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
        entity.Property(e => e.IsAnonymous).HasDefaultValue(false).HasColumnName("is_anonymous");
        entity.Property(e => e.SearchVector).HasColumnName("search_vector");
        entity.Property(e => e.Status).HasMaxLength(20).HasDefaultValueSql("'active'::character varying").HasColumnName("status");
        entity.Property(e => e.SubjectId).HasColumnName("subject_id");
        entity.Property(e => e.Title).HasMaxLength(255).HasColumnName("title");
        entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()").HasColumnName("updated_at");
        entity.Property(e => e.Upvotes).HasDefaultValue(0).HasColumnName("upvotes");
        entity.Property(e => e.ViewCount).HasDefaultValue(0).HasColumnName("view_count");
        entity.HasOne(d => d.Author).WithMany(p => p.Posts).HasForeignKey(d => d.AuthorId).HasConstraintName("posts_author_id_fkey");
        entity.HasOne(d => d.Subject).WithMany(p => p.Posts).HasForeignKey(d => d.SubjectId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("posts_subject_id_fkey");
    }
}