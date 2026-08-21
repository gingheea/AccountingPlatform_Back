using Accounting.Application.Common;
using Accounting.Application.Features.ClientSubscriptions.Common;
using Accounting.Domain.Enums;
using MediatR;
using System;

namespace Accounting.Application.Features.ClientSubscriptions.ListSubscriptions
{
    /// <param name="UserId">
    /// The portal always fills this from the token, so a client cannot see others'.
    /// The admin endpoint passes null, or a specific client to filter by.
    /// </param>
    public sealed record ListClientSubscriptionsQuery(
        Guid? UserId,
        SubscriptionStatus? Status,
        int Page,
        int PageSize
    ) : IRequest<PagedResult<ClientSubscriptionDto>>;
}
