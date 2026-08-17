using Accounting.Domain.Exceptions;
using Accounting.Domain.ValueObjects;

namespace Accounting.Domain.Entities;

/// <summary>
/// Підписник на розсилку. Зберігаємо у себе, навіть попри те що список
/// дублюється в Brevo: свою базу контактів не можна втратити разом з
/// доступом до чужого сервісу.
/// </summary>
public sealed class NewsletterSubscriber
{
    public Guid Id { get; private set; }

    public string Email { get; private set; } = string.Empty;

    /// <summary>Звідки підписались — футер, головна, блог. Видно, що працює.</summary>
    public string Source { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }

    public DateTime SubscribedAtUtc { get; private set; }
    public DateTime? UnsubscribedAtUtc { get; private set; }

    private NewsletterSubscriber() { }

    public static NewsletterSubscriber Create(string email, string? source)
    {
        return new NewsletterSubscriber
        {
            Id = Guid.NewGuid(),
            // Перевірку формату не дублюємо — вона вже живе в Email.
            Email = ValueObjects.Email.Create(email).Value.ToLowerInvariant(),
            Source = NormalizeSource(source),
            IsActive = true,
            SubscribedAtUtc = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Повторна підписка того, хто раніше відписався. Новий запис не створюємо:
    /// пошта має бути в базі один раз.
    /// </summary>
    public void Resubscribe(string? source)
    {
        IsActive = true;
        UnsubscribedAtUtc = null;
        SubscribedAtUtc = DateTime.UtcNow;
        Source = NormalizeSource(source);
    }

    public void Unsubscribe()
    {
        if (!IsActive)
            throw new DomainException("Subscriber is already unsubscribed.");

        IsActive = false;
        UnsubscribedAtUtc = DateTime.UtcNow;
    }

    private static string NormalizeSource(string? source)
    {
        source = source?.Trim();

        if (string.IsNullOrWhiteSpace(source))
            return "unknown";

        return source.Length > 50 ? source[..50] : source;
    }
}
