using MediatR;
using System;

namespace Accounting.Application.Features.ClientSubscriptions.CreateSubscription
{
    public sealed record CreateClientSubscriptionCommand(
        Guid UserId,
        Guid? ServiceId,
        Guid? PricingPackageId,
        DateTime StartedAtUtc,
        string? Note
    ) : IRequest<Guid>;
}
