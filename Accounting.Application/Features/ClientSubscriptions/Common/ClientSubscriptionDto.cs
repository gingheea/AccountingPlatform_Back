using Accounting.Domain.Enums;
using System;

namespace Accounting.Application.Features.ClientSubscriptions.Common
{
    /// <param name="ServiceName">
    /// Назва підставляється в обробнику: кабінет має показувати «Повний
    /// супровід», а не набір символів ідентифікатора.
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
