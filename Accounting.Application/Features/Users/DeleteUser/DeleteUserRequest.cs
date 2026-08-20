using MediatR;
using System;

namespace Accounting.Application.Features.Users.DeleteUser
{
    public sealed record DeleteUserRequest(Guid Id) : IRequest;
}
