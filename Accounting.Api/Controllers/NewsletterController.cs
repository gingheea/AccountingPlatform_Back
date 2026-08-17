using Accounting.Api.Common;
using Accounting.Api.Contracts.Newsletter;
using Accounting.Application.Features.Newsletter.ListSubscribers;
using Accounting.Application.Features.Newsletter.RemoveSubscriber;
using Accounting.Application.Features.Newsletter.Subscribe;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Accounting.Api.Controllers
{
    [ApiController]
    [Route("api/newsletter")]
    public class NewsletterController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NewsletterController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Публічний і анонімний, як і форма заявки, тому під тим самим
        /// обмеженням частоти — інакше це другий вхід для спаму.
        /// </summary>
        [HttpPost("subscribe")]
        [AllowAnonymous]
        [EnableRateLimiting(RateLimitPolicies.PublicNewsletter)]
        public async Task<ActionResult> Subscribe(
            [FromBody] SubscribeRequest request,
            CancellationToken ct)
        {
            await _mediator.Send(new SubscribeToNewsletterCommand(request.Email, request.Source), ct);

            return NoContent();
        }

        /// <summary>Список підписників — щоб вони не були видимі лише в Brevo.</summary>
        [HttpGet("subscribers")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(IReadOnlyList<NewsletterSubscriberDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<NewsletterSubscriberDto>>> Subscribers(
            [FromQuery] bool? isActive,
            CancellationToken ct)
        {
            var subscribers = await _mediator.Send(new ListSubscribersQuery(isActive), ct);

            return Ok(subscribers);
        }

        [HttpDelete("subscribers/{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> RemoveSubscriber(Guid id, CancellationToken ct)
        {
            await _mediator.Send(new RemoveSubscriberCommand(id), ct);

            return NoContent();
        }
    }
}
