using Accounting.Domain.Enums;
using Accounting.Domain.Exceptions;

namespace Accounting.Domain.Entities;

/// <summary>
/// A client testimonial. It reaches the site only after the accountant approves
/// it: the testimonials page is a shop window, and publishing anything there
/// unreviewed is not an option.
/// </summary>
public sealed class Testimonial
{
    public const int MinContentLength = 20;
    public const int MaxContentLength = 1000;

    public Guid Id { get; private set; }

    /// <summary>The author. Tied to an account: only a client can leave a testimonial.</summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// The name as of writing. A copy, not a reference: if the client later renames
    /// their profile, an already published testimonial must not silently change
    /// its signature.
    /// </summary>
    public string AuthorName { get; private set; } = string.Empty;

    /// <summary>The author's occupation, e.g. "sole trader", "shop owner". Optional.</summary>
    public string? AuthorRole { get; private set; }

    public string Content { get; private set; } = string.Empty;

    public TestimonialStatus Status { get; private set; }

    /// <summary>Rejection reason, so the client can see what to fix.</summary>
    public string? ModerationNote { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? ModeratedAtUtc { get; private set; }

    // EF Core needs a parameterless constructor
    private Testimonial() { }

    public static Testimonial Create(Guid userId, string authorName, string? authorRole, string content)
    {
        if (userId == Guid.Empty)
            throw new DomainException("User id is required.");

        return new Testimonial
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            AuthorName = NormalizeName(authorName),
            AuthorRole = NormalizeRole(authorRole),
            Content = NormalizeContent(content),
            Status = TestimonialStatus.Pending,
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    /// <summary>Editing is allowed while the testimonial is pending or rejected.</summary>
    public void UpdateContent(string content, string? authorRole)
    {
        if (Status == TestimonialStatus.Approved)
            throw new DomainException("An approved testimonial can no longer be edited.");

        Content = NormalizeContent(content);
        AuthorRole = NormalizeRole(authorRole);

        // An edited testimonial goes back for review: otherwise rejected text could be
        // swapped for anything and slip past moderation.
        Status = TestimonialStatus.Pending;
        ModerationNote = null;
        ModeratedAtUtc = null;
    }

    public void Approve()
    {
        if (Status == TestimonialStatus.Approved)
            throw new DomainException("Testimonial is already approved.");

        Status = TestimonialStatus.Approved;
        ModerationNote = null;
        ModeratedAtUtc = DateTime.UtcNow;
    }

    public void Reject(string? note)
    {
        if (Status == TestimonialStatus.Rejected)
            throw new DomainException("Testimonial is already rejected.");

        Status = TestimonialStatus.Rejected;
        ModerationNote = NormalizeNote(note);
        ModeratedAtUtc = DateTime.UtcNow;
    }

    private static string NormalizeName(string name)
    {
        name = name?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Author name is required.");

        return name.Length > 150 ? name[..150] : name;
    }

    private static string? NormalizeRole(string? role)
    {
        role = role?.Trim();

        if (string.IsNullOrWhiteSpace(role))
            return null;

        return role.Length > 100 ? role[..100] : role;
    }

    private static string NormalizeContent(string content)
    {
        content = content?.Trim() ?? string.Empty;

        // The validator checks length too, but the rule belongs to the entity:
        // that way it cannot be bypassed, whichever side calls in.
        if (content.Length < MinContentLength)
            throw new DomainException($"Testimonial must be at least {MinContentLength} characters long.");

        if (content.Length > MaxContentLength)
            throw new DomainException($"Testimonial max length is {MaxContentLength}.");

        return content;
    }

    private static string? NormalizeNote(string? note)
    {
        note = note?.Trim();

        if (string.IsNullOrWhiteSpace(note))
            return null;

        return note.Length > 500 ? note[..500] : note;
    }
}
