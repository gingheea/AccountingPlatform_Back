using Accounting.Domain.Enums;
using MediatR;
using System;

namespace Accounting.Application.Features.ClientRequests.Events
{
    /// <summary>
    /// The fact that a request was created. Published after the database save.
    /// Who reacts is none of the publisher's business: one email today,
    /// Telegram tomorrow, and CreateClientRequestHandler stays unchanged.
    /// </summary>
    public sealed record ClientRequestCreatedNotification(
        Guid RequestId,
        string FullName,
        string Email,
        string? Phone,
        string? Message,
        RequestType RequestType,
        Guid? ServiceId,
        Guid? PricingPackageId,
        DateTime CreatedAtUtc
    ) : INotification;
}
