using Anonwork.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anonwork.Infrastructure.Persistence.Configuration;

public class EmailVerificationTokenConfiguration : IEntityTypeConfiguration<EmailVerificationToken>
{
    public void Configure(EntityTypeBuilder<EmailVerificationToken> entity)
    {
        entity.HasKey(e => e.Id).HasName("email_verification_tokens_pkey");
        entity.ToTable("email_verification_tokens");
        entity.HasIndex(e => e.Email, "idx_email_verification_tokens_email");
        entity.HasIndex(e => e.TokenHash, "email_verification_tokens_token_hash_key").IsUnique();

        entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()").HasColumnName("id");
        entity.Property(e => e.Email).HasMaxLength(255).HasColumnName("email");
        entity.Property(e => e.Username).HasMaxLength(50).HasColumnName("username");
        entity.Property(e => e.TokenHash).HasMaxLength(255).HasColumnName("token_hash");
        entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
        entity.Property(e => e.VerifiedAt).HasColumnName("verified_at");
        entity.Property(e => e.IsUsed).HasDefaultValue(false).HasColumnName("is_used");
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");
    }
}