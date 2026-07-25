using Anonwork.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anonwork.Infrastructure.Persistence.Configuration;

public class UserActivityLogConfiguration : IEntityTypeConfiguration<UserActivityLog>
{
    public void Configure(EntityTypeBuilder<UserActivityLog> entity)
    {
        entity.HasKey(e => e.Id).HasName("user_activity_logs_pkey");
        entity.ToTable("user_activity_logs");

        entity.HasIndex(e => e.UserId, "idx_user_activity_logs_user_id");
        entity.HasIndex(e => e.Action, "idx_user_activity_logs_action");
        entity.HasIndex(e => e.CreatedAt, "idx_user_activity_logs_created_at");

        entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()").HasColumnName("id");
        entity.Property(e => e.UserId).HasColumnName("user_id");
        entity.Property(e => e.Action).HasMaxLength(100).HasColumnName("action");
        entity.Property(e => e.ActionCategory).HasMaxLength(50).HasColumnName("action_category");
        entity.Property(e => e.Description).HasMaxLength(500).HasColumnName("description");
        entity.Property(e => e.TargetType).HasMaxLength(50).HasColumnName("target_type");
        entity.Property(e => e.TargetId).HasMaxLength(100).HasColumnName("target_id");
        entity.Property(e => e.IpAddress).HasMaxLength(45).HasColumnName("ip_address");
        entity.Property(e => e.UserAgent).HasMaxLength(500).HasColumnName("user_agent");
        entity.Property(e => e.DetailsJson).HasColumnName("details_json");
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");

        entity.HasOne(d => d.User)
            .WithMany()
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("user_activity_logs_user_id_fkey");
    }
}
