using Accounting.Domain.Enums;
using MediatR;
using System;

namespace Accounting.Application.Features.ClientRequests.Events
{
    /// <summary>
    /// Факт «заявку створено». Публікується після збереження в базу.
    /// Хто на це реагує — не турбота того, хто публікує: сьогодні один лист,
    /// завтра ще Telegram, і CreateClientRequestHandler не зміниться.
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
