using Accounting.Domain.Enums;
using System;

namespace Accounting.Application.Features.Testimonials.Common
{
    /// <summary>
    /// What a site visitor sees. No UserId, no status, no internal notes: only
    /// what genuinely needs to be shown goes out. An extra field in a DTO is
    /// a data leak even when the frontend never renders it.
    /// </summary>
    public sealed record PublicTestimonialDto(
        Guid Id,
        string AuthorName,
        string? AuthorRole,
        string Content,
        DateTime CreatedAtUtc
    );

    /// <summary>The full view: for the admin panel and for the author's own portal.</summary>
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
