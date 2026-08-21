using Accounting.Application.Common;
using Accounting.Application.Features.ClientSubscriptions.Common;
using Accounting.Domain.Enums;
using MediatR;
using System;

namespace Accounting.Application.Features.ClientSubscriptions.ListSubscriptions
{
    /// <param name="UserId">
    /// Портал завжди підставляє сюди id з токена, тож клієнт не побачить чуже.
    /// Адмінський ендпоінт передає null або конкретного клієнта для фільтра.
    /// </param>
    public sealed record ListClientSubscriptionsQuery(
        Guid? UserId,
        SubscriptionStatus? Status,
        int Page,
        int PageSize
    ) : IRequest<PagedResult<ClientSubscriptionDto>>;
}
