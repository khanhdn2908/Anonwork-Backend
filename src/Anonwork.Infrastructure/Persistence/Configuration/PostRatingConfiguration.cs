using Anonwork.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anonwork.Infrastructure.Persistence.Configuration;

public class PostRatingConfiguration : IEntityTypeConfiguration<PostRating>
{
    public void Configure(EntityTypeBuilder<PostRating> entity)
    {
        entity.HasKey(e => e.Id).HasName("post_ratings_pkey");
        entity.ToTable("post_ratings");

        entity.HasIndex(e => new { e.PostId, e.UserId }, "post_ratings_post_id_user_id_key").IsUnique();
        entity.HasIndex(e => e.PostId, "idx_post_ratings_post");

        entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()").HasColumnName("id");
        entity.Property(e => e.PostId).HasColumnName("post_id");
        entity.Property(e => e.UserId).HasColumnName("user_id");
        entity.Property(e => e.Stars).HasColumnName("stars");
        entity.Property(e => e.Review).HasMaxLength(500).HasColumnName("review");
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");
        entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()").HasColumnName("updated_at");

        entity.HasOne(d => d.Post)
            .WithMany(p => p.PostRatings)
            .HasForeignKey(d => d.PostId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("post_ratings_post_id_fkey");

        entity.HasOne(d => d.User)
            .WithMany()
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("post_ratings_user_id_fkey");
    }
}
