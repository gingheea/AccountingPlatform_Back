using Accounting.Api.Contracts.ClientSubscriptions;
using Accounting.Application.Features.ClientSubscriptions.ChangeStatus;
using Accounting.Application.Features.ClientSubscriptions.Common;
using Accounting.Application.Features.ClientSubscriptions.CreateSubscription;
using Accounting.Application.Features.ClientSubscriptions.DeleteSubscription;
using Accounting.Application.Features.ClientSubscriptions.ListSubscriptions;
using Accounting.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.Api.Controllers
{
    [ApiController]
    [Route("api/client-subscriptions")]
    [Authorize(Roles = "Admin")]
    public class ClientSubscriptionsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ClientSubscriptionsController> _logger;

        public ClientSubscriptionsController(
            IMediator mediator,
            ILogger<ClientSubscriptionsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<ClientSubscriptionDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<ClientSubscriptionDto>>> List(
            [FromQuery] Guid? userId,
            [FromQuery] SubscriptionStatus? status,
            CancellationToken ct)
        {
            var subscriptions = await _mediator.Send(new ListClientSubscriptionsQuery(userId, status), ct);

            return Ok(subscriptions);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Create(
            [FromBody] CreateClientSubscriptionRequest request,
            CancellationToken ct)
        {
            _logger.LogInformation("Creating subscription for user {UserId}", request.UserId);

            var id = await _mediator.Send(
                new CreateClientSubscriptionCommand(
                    request.UserId,
                    request.ServiceId,
                    request.PricingPackageId,
                    request.StartedAtUtc,
                    request.Note),
                ct);

            return Ok(new { id });
        }

        [HttpPatch("{id:guid}/status")]
        public async Task<ActionResult> ChangeStatus(
            Guid id,
            [FromBody] ChangeSubscriptionStatusRequest request,
            CancellationToken ct)
        {
            await _mediator.Send(new ChangeSubscriptionStatusCommand(id, request.Status), ct);

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> Delete(Guid id, CancellationToken ct)
        {
            await _mediator.Send(new DeleteClientSubscriptionCommand(id), ct);

            return NoContent();
        }
    }
}
