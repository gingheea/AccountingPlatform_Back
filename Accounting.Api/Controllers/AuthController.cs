using Accounting.Api.Contracts.Auth;
using Accounting.Application.Features.Auth.Login;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new Command(request.Email, request.Password), ct);
        return Ok(new AuthResponse(result.AccessToken));
    }
}
