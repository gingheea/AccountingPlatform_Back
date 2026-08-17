using Accounting.Domain.Enums;
using MediatR;
using System;

namespace Accounting.Application.Features.ClientSubscriptions.ChangeStatus
{
    public sealed record ChangeSubscriptionStatusCommand(
        Guid Id,
        SubscriptionStatus Status
    ) : IRequest;
}
