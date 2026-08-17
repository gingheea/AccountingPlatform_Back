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
    /// Завжди 204, навіть якщо такої пошти немає. Різна відповідь дозволила б
    /// перебирати адреси й дізнаватись, хто зареєстрований.
    /// Під обмеженням частоти, бо кожен виклик шле лист.
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
