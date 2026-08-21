using Accounting.Domain.Enums;
using System;

namespace Accounting.Application.Features.ClientSubscriptions.Common
{
    /// <param name="ServiceName">
    /// The name is filled in by the handler: the portal must show "Full support"
    /// rather than a string of identifier characters.
    /// </param>
    public sealed record ClientSubscriptionDto(
        Guid Id,
        Guid UserId,
        Guid? ServiceId,
        Guid? PricingPackageId,
        string? ServiceName,
        string? PricingPackageName,
        SubscriptionStatus Status,
        DateTime StartedAtUtc,
        DateTime? EndedAtUtc,
        string? Note,
        DateTime CreatedAtUtc,
        DateTime UpdatedAtUtc
    );
}
