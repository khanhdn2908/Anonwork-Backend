using Anonwork.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anonwork.Infrastructure.Persistence.Configuration;

public class FollowConfiguration : IEntityTypeConfiguration<Follow>
{
    public void Configure(EntityTypeBuilder<Follow> entity)
    {
        entity.HasKey(e => e.Id).HasName("follows_pkey");
        entity.ToTable("follows");
        entity.HasIndex(e => e.FollowingId, "idx_follows_following");
        entity.HasIndex(e => new { e.FollowerId, e.FollowingId }, "uq_follows").IsUnique();
        entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()").HasColumnName("id");
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");
        entity.Property(e => e.FollowerId).HasColumnName("follower_id");
        entity.Property(e => e.FollowingId).HasColumnName("following_id");
        entity.HasOne(d => d.Follower).WithMany(p => p.FollowFollowers).HasForeignKey(d => d.FollowerId).HasConstraintName("follows_follower_id_fkey");
        entity.HasOne(d => d.Following).WithMany(p => p.FollowFollowings).HasForeignKey(d => d.FollowingId).HasConstraintName("follows_following_id_fkey");
    }
}