using MediatR;
using System;

namespace Accounting.Application.Features.ClientRequests.DeleteClientRequest
{
    public sealed record DeleteClientRequestCommand(Guid Id) : IRequest;
}
