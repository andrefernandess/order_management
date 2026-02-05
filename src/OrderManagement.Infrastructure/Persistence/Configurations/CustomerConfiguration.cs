
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        // Primary Key
        builder.HasKey(c => c.Id);

        // Properties
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        // Value Object - Email (Owned Type)
        builder.OwnsOne(c => c.Email, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("Email")
                .IsRequired()
                .HasMaxLength(256);

            email.HasIndex(e => e.Value).IsUnique();
        });

        builder.Property(c => c.Phone)
            .HasMaxLength(20);

        builder.Property(c => c.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Value Object - Address (Owned Type)
        builder.OwnsOne(c => c.Address, address =>
        {
            address.Property(a => a.Street)
                .HasColumnName("Address_Street")
                .HasMaxLength(200);

            address.Property(a => a.Number)
                .HasColumnName("Address_Number")
                .HasMaxLength(20);

            address.Property(a => a.Complement)
                .HasColumnName("Address_Complement")
                .HasMaxLength(100);

            address.Property(a => a.Neighborhood)
                .HasColumnName("Address_Neighborhood")
                .HasMaxLength(100);

            address.Property(a => a.City)
                .HasColumnName("Address_City")
                .HasMaxLength(100);

            address.Property(a => a.State)
                .HasColumnName("Address_State")
                .HasMaxLength(2);

            address.Property(a => a.ZipCode)
                .HasColumnName("Address_ZipCode")
                .HasMaxLength(10);

            address.Property(a => a.Country)
                .HasColumnName("Address_Country")
                .HasMaxLength(50);
        });

        // Audit fields
        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.CreatedBy)
            .HasMaxLength(100);

        builder.Property(c => c.UpdatedBy)
            .HasMaxLength(100);

        // Indexes
        builder.HasIndex(c => c.IsActive);
    }
}
