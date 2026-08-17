using Accounting.Application.Features.ClientSubscriptions.Common;
using Accounting.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;

namespace Accounting.Application.Features.ClientSubscriptions.ListSubscriptions
{
    /// <param name="UserId">
    /// Портал завжди підставляє сюди id з токена, тож клієнт не побачить чуже.
    /// Адмінський ендпоінт передає null або конкретного клієнта для фільтра.
    /// </param>
    public sealed record ListClientSubscriptionsQuery(
        Guid? UserId,
        SubscriptionStatus? Status
    ) : IRequest<IReadOnlyList<ClientSubscriptionDto>>;
}
