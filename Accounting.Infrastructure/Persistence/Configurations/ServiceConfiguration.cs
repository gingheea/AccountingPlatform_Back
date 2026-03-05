using Accounting.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Accounting.Infrastructure.Persistence.Configurations;

public sealed class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> b)
    {
        b.ToTable("Services");

        b.HasKey(x => x.Id);

        b.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        b.Property(x => x.Description)
            .HasMaxLength(2000);

        b.Property(x => x.Price)
            .HasColumnType("numeric(12,2)");

        b.Property(x => x.IsActive)
            .IsRequired();

        b.Property(x => x.SortOrder)
            .IsRequired();

        b.HasIndex(x => x.IsActive);
        b.HasIndex(x => x.SortOrder);
    }
}