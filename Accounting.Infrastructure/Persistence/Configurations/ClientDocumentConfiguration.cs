using Accounting.Domain.Entities;
using Accounting.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Accounting.Infrastructure.Persistence.Configurations
{
    internal class ClientDocumentConfiguration : IEntityTypeConfiguration<ClientDocument>
    {
        public void Configure(EntityTypeBuilder<ClientDocument> b)
        {
            b.ToTable("ClientDocuments");

            b.HasKey(x => x.Id);

            b.Property(x => x.UserId)
                .IsRequired();

            b.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);

            b.Property(x => x.FileName)
                .IsRequired()
                .HasMaxLength(255);

            b.Property(x => x.StorageKey)
                .IsRequired()
                .HasMaxLength(500);

            b.Property(x => x.ContentType)
                .IsRequired()
                .HasMaxLength(150);

            b.Property(x => x.SizeBytes)
                .IsRequired();

            b.Property(x => x.Category)
                .IsRequired();

            b.Property(x => x.Direction)
                .IsRequired();

            b.Property(x => x.Status)
                .IsRequired();

            b.Property(x => x.Note)
                .HasMaxLength(2000);

            b.Property(x => x.CreatedAtUtc)
                .IsRequired();

            b.Property(x => x.UpdatedAtUtc)
                .IsRequired();

            b.HasOne<AppUser>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasIndex(x => x.UserId);
            b.HasIndex(x => x.Status);
            b.HasIndex(x => x.Category);
            b.HasIndex(x => x.Direction);
            b.HasIndex(x => x.CreatedAtUtc);

            b.HasIndex(x => x.StorageKey)
                .IsUnique();
        }
    }
}
