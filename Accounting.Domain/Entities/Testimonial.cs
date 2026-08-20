using Accounting.Domain.Enums;
using Accounting.Domain.Exceptions;

namespace Accounting.Domain.Entities;

/// <summary>
/// Відгук клієнта. На сайт потрапляє лише після схвалення бухгалтером —
/// сторінка з відгуками це вітрина, і публікувати туди що завгодно
/// без перегляду не можна.
/// </summary>
public sealed class Testimonial
{
    public const int MinContentLength = 20;
    public const int MaxContentLength = 1000;

    public Guid Id { get; private set; }

    /// <summary>Автор. Прив'язка до акаунта — відгук може лишити тільки клієнт.</summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Ім'я на момент написання. Це копія, а не посилання: якщо клієнт потім
    /// змінить ім'я в профілі, вже опублікований відгук не має мовчки
    /// підписатись інакше.
    /// </summary>
    public string AuthorName { get; private set; } = string.Empty;

    /// <summary>Хто автор за родом занять — «ФОП», «власниця магазину». Необов'язково.</summary>
    public string? AuthorRole { get; private set; }

    public string Content { get; private set; } = string.Empty;

    public TestimonialStatus Status { get; private set; }

    /// <summary>Причина відхилення — щоб клієнт бачив, що виправити.</summary>
    public string? ModerationNote { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? ModeratedAtUtc { get; private set; }

    // EF Core потребує безпараметричний конструктор
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

    /// <summary>Правка тексту доступна, поки відгук не розглянули або його відхилили.</summary>
    public void UpdateContent(string content, string? authorRole)
    {
        if (Status == TestimonialStatus.Approved)
            throw new DomainException("An approved testimonial can no longer be edited.");

        Content = NormalizeContent(content);
        AuthorRole = NormalizeRole(authorRole);

        // Правлений відгук знову йде на розгляд: інакше відхилений текст можна
        // було б замінити будь-чим і обійти перевірку.
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

        // Довжину перевіряє ще й валідатор, але правило належить сутності:
        // так його не обійти, з якого боку до неї не звернутись.
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
