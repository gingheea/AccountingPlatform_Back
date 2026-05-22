using Accounting.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Infrastructure.Persistence.Configurations
{
    internal class PricingPackageConfiguration : IEntityTypeConfiguration<PricingPackage>
    {
        public void Configure(EntityTypeBuilder<PricingPackage> b)
        {
            b.ToTable("PricingPackages");

            b.HasKey(x => x.Id);

            b.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            b.Property(x => x.Badge)
                .HasMaxLength(100);

            b.Property(x => x.Description)
                .HasMaxLength(1000);

            b.Property(x => x.Price)
                .HasColumnType("numeric(12,2)")
                .IsRequired();

            b.Property(x => x.PriceLabel)
                .HasMaxLength(100);

            b.Property(x => x.PeriodLabel)
                .HasMaxLength(100);

            b.Property(x => x.Features)
                .HasColumnType("text[]");

            b.Property(x => x.IsRecommended)
                .IsRequired();

            b.Property(x => x.IsActive)
                .IsRequired();

            b.Property(x => x.SortOrder)
                .IsRequired();

            b.HasIndex(x => x.IsActive);
            b.HasIndex(x => x.SortOrder);
        }
    }
}
