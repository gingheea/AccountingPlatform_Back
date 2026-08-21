using Accounting.Api.Common;
using Accounting.Api.Contracts.Auth;
using Accounting.Application.Features.Auth.ForgotPassword;
using Accounting.Application.Features.Auth.ResetPassword;
using Login = Accounting.Application.Features.Auth.Login;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

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
        var result = await _mediator.Send(new Login.Command(request.Email, request.Password), ct);
        return Ok(new AuthResponse(result.AccessToken));
    }

    /// <summary>
    /// Always 204, even when the address is unknown. Differing responses would let
    /// someone enumerate addresses and learn who is registered.
    /// Rate limited, because every call sends an email.
    /// </summary>
    [HttpPost("forgot-password")]
    [EnableRateLimiting(RateLimitPolicies.PublicNewsletter)]
    public async Task<ActionResult> ForgotPassword(
        [FromBody] ForgotPasswordRequest request,
        CancellationToken ct)
    {
        await _mediator.Send(new ForgotPasswordCommand(request.Email), ct);

        return NoContent();
    }

    [HttpPost("reset-password")]
    [EnableRateLimiting(RateLimitPolicies.PublicNewsletter)]
    public async Task<ActionResult> ResetPassword(
        [FromBody] ResetPasswordRequest request,
        CancellationToken ct)
    {
        await _mediator.Send(
            new ResetPasswordCommand(request.Email, request.Token, request.NewPassword), ct);

        return NoContent();
    }
}
