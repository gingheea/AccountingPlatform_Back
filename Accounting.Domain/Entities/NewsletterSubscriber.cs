using Accounting.Domain.Exceptions;
using Accounting.Domain.ValueObjects;

namespace Accounting.Domain.Entities;

/// <summary>
/// A newsletter subscriber. Kept on our side even though the list is also
/// mirrored in Brevo: our own contact base must not be lost together with
/// access to somebody else's service.
/// </summary>
public sealed class NewsletterSubscriber
{
    public Guid Id { get; private set; }

    public string Email { get; private set; } = string.Empty;

    /// <summary>Where the signup came from: footer, home, blog. Shows what works.</summary>
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
            // Format checking is not duplicated here: it already lives in Email.
            Email = ValueObjects.Email.Create(email).Value.ToLowerInvariant(),
            Source = NormalizeSource(source),
            IsActive = true,
            SubscribedAtUtc = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Re-subscribing someone who opted out earlier. No new row is created:
    /// an address must appear in the database exactly once.
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
