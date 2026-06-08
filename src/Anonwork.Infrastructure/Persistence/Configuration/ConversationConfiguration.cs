using Anonwork.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anonwork.Infrastructure.Persistence.Configuration;

public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> entity)
    {
        entity.HasKey(e => e.Id).HasName("conversations_pkey");
        entity.ToTable("conversations");
        entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()").HasColumnName("id");
        entity.Property(e => e.AvatarUrl).HasColumnName("avatar_url");
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");
        entity.Property(e => e.IsActive).HasDefaultValue(true).HasColumnName("is_active");
        entity.Property(e => e.IsGroup).HasDefaultValue(false).HasColumnName("is_group");
        entity.Property(e => e.Title).HasMaxLength(100).HasColumnName("title");
        entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()").HasColumnName("updated_at");
    }
}