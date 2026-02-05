
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Infrastructure.Persistence.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");

        // Primary Key
        builder.HasKey(i => i.Id);

        // Properties
        builder.Property(i => i.ProductId)
            .IsRequired()
            .HasMaxLength(24); // MongoDB ObjectId length

        builder.Property(i => i.ProductName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(i => i.Quantity)
            .IsRequired();

        // Value Object - UnitPrice (Owned Type)
        builder.OwnsOne(i => i.UnitPrice, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("UnitPrice")
                .HasPrecision(18, 2)
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("UnitPrice_Currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        // Ignore calculated property
        builder.Ignore(i => i.TotalPrice);

        // Ignore DomainEvents
        builder.Ignore(i => i.DomainEvents);

        // Audit
        builder.Property(i => i.CreatedAt).IsRequired();

        // Index
        builder.HasIndex(i => i.OrderId);
        builder.HasIndex(i => i.ProductId);
    }
}
