using Accounting.Domain.Enums;
using System;

namespace Accounting.Application.Features.Testimonials.Common
{
    /// <summary>
    /// Те, що бачить відвідувач сайту. Ні UserId, ні статусу, ні службових
    /// нотаток тут немає — назовні віддаємо тільки те, що справді потрібно
    /// показати. Зайве поле в DTO це витік даних, навіть якщо фронт його
    /// не малює.
    /// </summary>
    public sealed record PublicTestimonialDto(
        Guid Id,
        string AuthorName,
        string? AuthorRole,
        string Content,
        DateTime CreatedAtUtc
    );

    /// <summary>Повний вигляд — для адмінки й для автора у власному кабінеті.</summary>
    public sealed record TestimonialDto(
        Guid Id,
        Guid UserId,
        string AuthorName,
        string? AuthorRole,
        string Content,
        TestimonialStatus Status,
        string? ModerationNote,
        DateTime CreatedAtUtc,
        DateTime? ModeratedAtUtc
    );
}
