using MediatR;
using System;

namespace Accounting.Application.Features.ClientSubscriptions.DeleteSubscription
{
    public sealed record DeleteClientSubscriptionCommand(Guid Id) : IRequest;
}
