using Accounting.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Accounting.Infrastructure.Persistence.Configurations
{
    internal class TestimonialConfiguration : IEntityTypeConfiguration<Testimonial>
    {
        public void Configure(EntityTypeBuilder<Testimonial> b)
        {
            b.ToTable("Testimonials");

            b.HasKey(x => x.Id);

            b.Property(x => x.UserId).IsRequired();
            b.Property(x => x.AuthorName).IsRequired().HasMaxLength(150);
            b.Property(x => x.AuthorRole).HasMaxLength(100);
            b.Property(x => x.Content).IsRequired().HasMaxLength(Testimonial.MaxContentLength);
            b.Property(x => x.Status).IsRequired().HasConversion<int>();
            b.Property(x => x.ModerationNote).HasMaxLength(500);
            b.Property(x => x.CreatedAtUtc).IsRequired();
            b.Property(x => x.ModeratedAtUtc);

            // Один відгук на клієнта. Унікальний індекс, а не перевірка в коді:
            // два одночасні запити не побачили б один одного і створили б дубль.
            b.HasIndex(x => x.UserId).IsUnique();

            // Публічна сторінка завжди питає «схвалені, найновіші зверху».
            b.HasIndex(x => new { x.Status, x.CreatedAtUtc });
        }
    }
}
