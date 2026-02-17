using Accounting.Application.Abstractions.Auth;
using MediatR;

namespace Accounting.Application.Features.Auth.Login;

public sealed class Handler : IRequestHandler<Command, Response>
{
    private readonly IIdentityAuthService _identity;
    private readonly IJwtTokenService _jwt;

    public Handler(IIdentityAuthService identity, IJwtTokenService jwt)
    {
        _identity = identity;
        _jwt = jwt;
    }

    public async Task<Response> Handle(Command request, CancellationToken ct)
    {
        var auth = await _identity.ValidateCredentialsAsync(request.Email, request.Password, ct);

        if (auth is null)
            throw new UnauthorizedAccessException("Invalid credentials");

        var (user, roles) = auth.Value;
        var token = _jwt.GenerateAccessToken(user, roles);

        return new Response(token);
    }
}
