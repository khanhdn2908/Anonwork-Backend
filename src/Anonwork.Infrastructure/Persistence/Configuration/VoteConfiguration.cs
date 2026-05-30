using Anonwork.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anonwork.Infrastructure.Persistence.Configuration;

public class VoteConfiguration : IEntityTypeConfiguration<Vote>
{
    public void Configure(EntityTypeBuilder<Vote> entity)
    {
        entity.HasKey(e => e.Id).HasName("votes_pkey");
        entity.ToTable("votes");
        entity.HasIndex(e => new { e.TargetId, e.TargetType }, "idx_votes_target");
        entity.HasIndex(e => new { e.UserId, e.TargetId, e.TargetType }, "votes_user_id_target_id_target_type_key").IsUnique();
        entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()").HasColumnName("id");
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");
        entity.Property(e => e.TargetId).HasColumnName("target_id");
        entity.Property(e => e.TargetType).HasMaxLength(10).HasColumnName("target_type");
        entity.Property(e => e.UserId).HasColumnName("user_id");
        entity.Property(e => e.VoteType).HasMaxLength(5).HasDefaultValueSql("'up'::character varying").HasColumnName("vote_type");
        entity.HasOne(d => d.User).WithMany(p => p.Votes).HasForeignKey(d => d.UserId).HasConstraintName("votes_user_id_fkey");
    }
}