using Anonwork.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anonwork.Infrastructure.Persistence.Configuration;

public class ConversationMemberConfiguration : IEntityTypeConfiguration<ConversationMember>
{
    public void Configure(EntityTypeBuilder<ConversationMember> entity)
    {
        entity.HasKey(e => new { e.ConversationId, e.UserId }).HasName("conversation_members_pkey");
        entity.ToTable("conversation_members");
        entity.Property(e => e.ConversationId).HasColumnName("conversation_id");
        entity.Property(e => e.UserId).HasColumnName("user_id");
        entity.Property(e => e.JoinedAt).HasDefaultValueSql("now()").HasColumnName("joined_at");
        entity.Property(e => e.LastReadAt).HasColumnName("last_read_at");
        entity.HasOne(d => d.Conversation).WithMany(p => p.ConversationMembers).HasForeignKey(d => d.ConversationId).HasConstraintName("conversation_members_conversation_id_fkey");
        entity.HasOne(d => d.User).WithMany(p => p.ConversationMembers).HasForeignKey(d => d.UserId).HasConstraintName("conversation_members_user_id_fkey");
    }
}