using Anonwork.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anonwork.Infrastructure.Persistence.Configuration;

public class ReportConfiguration : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> entity)
    {
        entity.HasKey(e => e.Id).HasName("reports_pkey");
        entity.ToTable("reports");
        entity.HasIndex(e => e.Status, "idx_reports_status");
        entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()").HasColumnName("id");
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");
        entity.Property(e => e.Reason).HasMaxLength(500).HasColumnName("reason");
        entity.Property(e => e.ReporterId).HasColumnName("reporter_id");
        entity.Property(e => e.Status).HasMaxLength(20).HasColumnName("status");
        entity.Property(e => e.TargetId).HasColumnName("target_id");
        entity.Property(e => e.TargetType).HasMaxLength(10).HasColumnName("target_type");
        entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()").HasColumnName("updated_at");
        entity.HasOne(d => d.Reporter).WithMany(p => p.Reports).HasForeignKey(d => d.ReporterId).HasConstraintName("reports_reporter_id_fkey");
    }
}