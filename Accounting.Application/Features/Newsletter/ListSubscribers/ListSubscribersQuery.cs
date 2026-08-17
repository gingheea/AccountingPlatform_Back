using MediatR;
using System;
using System.Collections.Generic;

namespace Accounting.Application.Features.Newsletter.ListSubscribers
{
    public sealed record NewsletterSubscriberDto(
        Guid Id,
        string Email,
        string Source,
        bool IsActive,
        DateTime SubscribedAtUtc,
        DateTime? UnsubscribedAtUtc
    );

    public sealed record ListSubscribersQuery(bool? IsActive)
        : IRequest<IReadOnlyList<NewsletterSubscriberDto>>;
}
