using Accounting.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Accounting.Infrastructure.Persistence.Configurations
{
    internal class NewsletterSubscriberConfiguration : IEntityTypeConfiguration<NewsletterSubscriber>
    {
        public void Configure(EntityTypeBuilder<NewsletterSubscriber> b)
        {
            b.ToTable("NewsletterSubscribers");

            b.HasKey(x => x.Id);

            b.Property(x => x.Email).IsRequired().HasMaxLength(200);
            b.Property(x => x.Source).IsRequired().HasMaxLength(50);
            b.Property(x => x.IsActive).IsRequired();
            b.Property(x => x.SubscribedAtUtc).IsRequired();
            b.Property(x => x.UnsubscribedAtUtc);

            // Унікальний індекс — щоб дубль не міг зʼявитись навіть тоді, коли
            // два запити прийшли одночасно й обидва не побачили один одного.
            b.HasIndex(x => x.Email).IsUnique();

            b.HasIndex(x => x.IsActive);
        }
    }
}
