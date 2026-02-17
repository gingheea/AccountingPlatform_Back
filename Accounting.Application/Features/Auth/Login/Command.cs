using MediatR;

namespace Accounting.Application.Features.Auth.Login;

public sealed record Command(string Email, string Password) : IRequest<Response>;
