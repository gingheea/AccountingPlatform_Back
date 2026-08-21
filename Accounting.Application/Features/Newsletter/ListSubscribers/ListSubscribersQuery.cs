using Accounting.Application.Common;
using MediatR;
using System;

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

    public sealed record ListSubscribersQuery(bool? IsActive, int Page, int PageSize)
        : IRequest<PagedResult<NewsletterSubscriberDto>>;
}
