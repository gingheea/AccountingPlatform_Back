using MediatR;
using System;

namespace Accounting.Application.Features.Newsletter.RemoveSubscriber
{
    /// <summary>
    /// A hard delete rather than an "unsubscribed" flag: when someone asks for
    /// their address to be removed, no row may stay behind.
    /// </summary>
    public sealed record RemoveSubscriberCommand(Guid Id) : IRequest;
}
