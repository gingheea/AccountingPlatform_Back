using Accounting.Application.Common;
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
        /// Public and anonymous like the request form, so it sits behind the same
        /// rate limit; otherwise it would be a second door for spam.
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

        /// <summary>The subscriber list, so it is not visible only inside Brevo.</summary>
        [HttpGet("subscribers")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(PagedResult<NewsletterSubscriberDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<NewsletterSubscriberDto>>> Subscribers(
            [FromQuery] bool? isActive,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = Pagination.DefaultPageSize,
            CancellationToken ct = default)
        {
            var subscribers = await _mediator.Send(new ListSubscribersQuery(isActive, page, pageSize), ct);

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
