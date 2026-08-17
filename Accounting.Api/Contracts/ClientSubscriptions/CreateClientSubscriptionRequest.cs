using System;

namespace Accounting.Api.Contracts.ClientSubscriptions
{
    public sealed record CreateClientSubscriptionRequest(
        Guid UserId,
        Guid? ServiceId,
        Guid? PricingPackageId,
        DateTime StartedAtUtc,
        string? Note
    );
}
