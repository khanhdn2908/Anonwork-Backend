using Anonwork.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anonwork.Infrastructure.Persistence.Configuration;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> entity)
    {
        entity.HasKey(e => e.Id).HasName("notifications_pkey");
        entity.ToTable("notifications");
        entity.HasIndex(e => new { e.UserId, e.IsRead, e.CreatedAt }, "idx_notifications_user").IsDescending(false, false, true);
        entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()").HasColumnName("id");
        entity.Property(e => e.ActorId).HasColumnName("actor_id");
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");
        entity.Property(e => e.IsRead).HasDefaultValue(false).HasColumnName("is_read");
        entity.Property(e => e.RefId).HasColumnName("ref_id");
        entity.Property(e => e.Type).HasMaxLength(20).HasColumnName("type");
        entity.Property(e => e.UserId).HasColumnName("user_id");
        entity.HasOne(d => d.Actor).WithMany(p => p.NotificationActors).HasForeignKey(d => d.ActorId).OnDelete(DeleteBehavior.SetNull).HasConstraintName("notifications_actor_id_fkey");
        entity.HasOne(d => d.User).WithMany(p => p.NotificationUsers).HasForeignKey(d => d.UserId).HasConstraintName("notifications_user_id_fkey");
    }
}