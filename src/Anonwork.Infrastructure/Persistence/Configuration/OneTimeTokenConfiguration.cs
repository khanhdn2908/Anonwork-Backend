using Anonwork.Domain.Entities;
using Anonwork.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anonwork.Infrastructure.Persistence.Configuration;

public class OneTimeTokenConfiguration : IEntityTypeConfiguration<OneTimeToken>
{
    public void Configure(EntityTypeBuilder<OneTimeToken> entity)
    {
        entity.HasKey(e => e.Id).HasName("one_time_tokens_pkey");
        entity.ToTable("one_time_tokens");

        entity.HasIndex(e => e.Email, "idx_one_time_tokens_email");
        entity.HasIndex(e => new { e.Email, e.Purpose }, "idx_one_time_tokens_email_purpose");
        entity.HasIndex(e => e.TokenHash, "one_time_tokens_token_hash_key").IsUnique();

        entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()").HasColumnName("id");
        entity.Property(e => e.Email).HasMaxLength(255).HasColumnName("email");
        entity.Property(e => e.Username).HasMaxLength(50).HasColumnName("username");
        entity.Property(e => e.Purpose)
            .HasConversion<string>()
            .HasMaxLength(50)
            .HasColumnName("purpose");
        entity.Property(e => e.TokenHash).HasMaxLength(255).HasColumnName("token_hash");
        entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
        entity.Property(e => e.UsedAt).HasColumnName("used_at");
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");
    }
}
