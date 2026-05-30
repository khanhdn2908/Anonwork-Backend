using Anonwork.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anonwork.Infrastructure.Persistence.Configuration;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> entity)
    {
        entity.HasKey(e => e.Id).HasName("messages_pkey");
        entity.ToTable("messages");
        entity.HasIndex(e => new { e.ConversationId, e.CreatedAt }, "idx_messages_conversation").IsDescending(false, true);
        entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()").HasColumnName("id");
        entity.Property(e => e.Content).HasColumnName("content");
        entity.Property(e => e.ConversationId).HasColumnName("conversation_id");
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");
        entity.Property(e => e.IsDeleted).HasDefaultValue(false).HasColumnName("is_deleted");
        entity.Property(e => e.SenderId).HasColumnName("sender_id");
        entity.HasOne(d => d.Conversation).WithMany(p => p.Messages).HasForeignKey(d => d.ConversationId).HasConstraintName("messages_conversation_id_fkey");
        entity.HasOne(d => d.Sender).WithMany(p => p.Messages).HasForeignKey(d => d.SenderId).HasConstraintName("messages_sender_id_fkey");
    }
}