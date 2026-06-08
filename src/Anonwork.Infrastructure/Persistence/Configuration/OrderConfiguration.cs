using Anonwork.Domain.Entities;
using Anonwork.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anonwork.Infrastructure.Persistence.Configuration;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> entity)
    {
        entity.HasKey(e => e.Id).HasName("orders_pkey");
        entity.ToTable("orders");
        entity.HasIndex(e => e.OrderCode, "idx_orders_order_code");
        entity.HasIndex(e => new { e.UserId, e.Status }, "idx_orders_user_status");
        entity.HasIndex(e => e.ExpiresAt, "idx_orders_expires_pending").HasFilter("status = 'Pending'");
        entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()").HasColumnName("id");
        entity.Property(e => e.UserId).HasColumnName("user_id");
        entity.Property(e => e.PlanId).HasColumnName("plan_id");
        entity.Property(e => e.OrderCode).HasMaxLength(50).HasColumnName("order_code");
        entity.Property(e => e.Amount).HasColumnName("amount");
        entity.Property(e => e.Currency).HasMaxLength(10).HasDefaultValueSql("'VND'::character varying").HasColumnName("currency");
        entity.Property(e => e.Status).HasConversion<string>().HasColumnName("status");
        entity.Property(e => e.PaymentMethod).HasConversion<string>().HasColumnName("payment_method");
        entity.Property(e => e.ProviderTransactionId).HasMaxLength(100).HasColumnName("sepay_transaction_id");
        entity.Property(e => e.Metadata).HasColumnType("jsonb").HasColumnName("metadata");
        entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
        entity.Property(e => e.PaidAt).HasColumnName("paid_at");
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");
        entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()").HasColumnName("updated_at");
        entity.HasOne(d => d.User).WithMany(p => p.Orders).HasForeignKey(d => d.UserId).HasConstraintName("orders_user_id_fkey");
        entity.HasOne(d => d.Plan).WithMany(p => p.Orders).HasForeignKey(d => d.PlanId).OnDelete(DeleteBehavior.SetNull).HasConstraintName("orders_plan_id_fkey");
    }
}