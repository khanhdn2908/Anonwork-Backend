using Anonwork.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anonwork.Infrastructure.Persistence.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> entity)
    {
        entity.HasKey(e => e.Id).HasName("users_pkey");
        entity.ToTable("users");
        entity.HasIndex(e => e.Username, "idx_users_username");
        entity.HasIndex(e => e.AnonAlias, "users_anon_alias_key").IsUnique();
        entity.HasIndex(e => e.Email, "users_email_key").IsUnique();
        entity.HasIndex(e => e.Username, "users_username_key").IsUnique();
        entity.HasIndex(e => e.GoogleSubject, "users_google_subject_key").IsUnique();
        entity.HasIndex(e => e.AnonImageId, "idx_users_anon_image_id");
        entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()").HasColumnName("id");
        entity.Property(e => e.AnonAlias).HasMaxLength(80).HasColumnName("anon_alias");
        entity.Property(e => e.AnonImageId).HasColumnName("anon_image_id");
        entity.Property(e => e.AvatarUrl).HasColumnName("avatar_url");
        entity.Property(e => e.Bio).HasColumnName("bio");
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");
        entity.Property(e => e.Email).HasMaxLength(255).HasColumnName("email");
        entity.Property(e => e.GoogleSubject).HasMaxLength(255).HasColumnName("google_subject");
        entity.Property(e => e.IsAnonDefault).HasDefaultValue(false).HasColumnName("is_anon_default");
        entity.Property(e => e.IsEmailVerified).HasDefaultValue(false).HasColumnName("is_email_verified");
        entity.Property(e => e.IsActive).HasDefaultValue(true).HasColumnName("is_active");
        entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
        entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()").HasColumnName("updated_at");
        entity.Property(e => e.Username).HasMaxLength(50).HasColumnName("username");

        entity.HasOne(d => d.AnonImage)
            .WithMany(p => p.Users)
            .HasForeignKey(d => d.AnonImageId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("users_anon_image_id_fkey");
    }
}
