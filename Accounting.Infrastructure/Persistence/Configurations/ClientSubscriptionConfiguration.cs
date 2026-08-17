using Accounting.Domain.Entities;
using Accounting.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Accounting.Infrastructure.Persistence.Configurations
{
    internal class ClientSubscriptionConfiguration : IEntityTypeConfiguration<ClientSubscription>
    {
        public void Configure(EntityTypeBuilder<ClientSubscription> b)
        {
            b.ToTable("ClientSubscriptions");

            b.HasKey(x => x.Id);

            b.Property(x => x.UserId).IsRequired();
            b.Property(x => x.Status).IsRequired();
            b.Property(x => x.StartedAtUtc).IsRequired();
            b.Property(x => x.EndedAtUtc);
            b.Property(x => x.Note).HasMaxLength(2000);
            b.Property(x => x.CreatedAtUtc).IsRequired();
            b.Property(x => x.UpdatedAtUtc).IsRequired();

            // Видаляється клієнт — зникає й запис про його обслуговування.
            b.HasOne<AppUser>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // А от послугу чи пакет видаляти, стираючи історію обслуговування,
            // не можна — тому лише обнуляємо посилання.
            b.HasOne<Service>()
                .WithMany()
                .HasForeignKey(x => x.ServiceId)
                .OnDelete(DeleteBehavior.SetNull);

            b.HasOne<PricingPackage>()
                .WithMany()
                .HasForeignKey(x => x.PricingPackageId)
                .OnDelete(DeleteBehavior.SetNull);

            b.HasIndex(x => x.UserId);
            b.HasIndex(x => x.Status);
            b.HasIndex(x => x.StartedAtUtc);
        }
    }
}
