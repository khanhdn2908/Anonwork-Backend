using Anonwork.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anonwork.Infrastructure.Persistence.Configuration;

public class BookmarkConfiguration : IEntityTypeConfiguration<Bookmark>
{
    public void Configure(EntityTypeBuilder<Bookmark> entity)
    {
        entity.HasKey(e => e.Id).HasName("bookmarks_pkey");
        entity.ToTable("bookmarks");
        entity.HasIndex(e => new { e.UserId, e.PostId }, "bookmarks_user_id_post_id_key").IsUnique();
        entity.HasIndex(e => new { e.UserId, e.CreatedAt }, "idx_bookmarks_user").IsDescending(false, true);
        entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()").HasColumnName("id");
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");
        entity.Property(e => e.PostId).HasColumnName("post_id");
        entity.Property(e => e.UserId).HasColumnName("user_id");
        entity.HasOne(d => d.Post).WithMany(p => p.Bookmarks).HasForeignKey(d => d.PostId).HasConstraintName("bookmarks_post_id_fkey");
        entity.HasOne(d => d.User).WithMany(p => p.Bookmarks).HasForeignKey(d => d.UserId).HasConstraintName("bookmarks_user_id_fkey");
    }
}