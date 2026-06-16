using Accounting.Domain.Entities;
using Accounting.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;


namespace Accounting.Infrastructure.Persistence.Configurations
{
    internal class ClientRequestConfiguration : IEntityTypeConfiguration<ClientRequest>
    {
        public void Configure(EntityTypeBuilder<ClientRequest> b)
        {
            b.ToTable("ClientRequests");

            b.HasKey(x => x.Id);

            b.Property(x => x.FullName)
                .IsRequired()
                .HasMaxLength(200);

            b.Property(x => x.Message)
                .HasMaxLength(4000);

            b.Property(x => x.Status)
                .IsRequired();

            b.Property(x => x.RequestType)
                .IsRequired();

            b.Property(x => x.Email)
                .IsRequired();

            b.Property(x => x.CreatedAtUtc)
                .IsRequired();

            b.Property(x => x.UpdatedAtUtc)
                .IsRequired();

            b.Property(x => x.ServiceId);

            b.Property(x => x.PricingPackageId);

            b.Property(x => x.UserId);

            b.HasOne<AppUser>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            b.HasOne<Service>()
            .WithMany()
            .HasForeignKey(x => x.ServiceId)
            .OnDelete(DeleteBehavior.SetNull);

            b.HasOne<PricingPackage>()
                .WithMany()
                .HasForeignKey(x => x.PricingPackageId)
                .OnDelete(DeleteBehavior.SetNull);

            b.HasIndex(x => x.ServiceId);
            b.HasIndex(x => x.PricingPackageId);
            b.HasIndex(x => x.Status);
            b.HasIndex(x => x.RequestType);
            b.HasIndex(x => x.CreatedAtUtc);
            b.HasIndex(x => x.UserId);


        }
    }
}
