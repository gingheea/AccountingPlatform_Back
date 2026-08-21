using Accounting.Domain.Entities;
using Accounting.Infrastructure.Identity;
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

            // Delete a client and their testimonial goes too. Without this relationship
            // the testimonial would linger, pointing at a user who no longer exists.
            b.HasOne<AppUser>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // One testimonial per client. A unique index rather than a check in code:
            // two simultaneous requests would not see each other and would both insert.
            b.HasIndex(x => x.UserId).IsUnique();

            // The public page always asks for "approved, newest first".
            b.HasIndex(x => new { x.Status, x.CreatedAtUtc });
        }
    }
}
