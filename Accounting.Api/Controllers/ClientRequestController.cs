using Accounting.Api.Contracts.ClientRequests;
using Accounting.Application.Features.ClientRequests.AssignClientRequestToUser;
using Accounting.Application.Features.ClientRequests.ChangeAdminNote;
using Accounting.Application.Features.ClientRequests.ChangeStatus;
using Accounting.Application.Features.ClientRequests.Common;
using Accounting.Application.Features.ClientRequests.Complete;
using Accounting.Application.Features.ClientRequests.CreateClientRequest;
using Accounting.Application.Features.ClientRequests.GetClientRequest;
using Accounting.Application.Features.ClientRequests.ListClientRequests;
using Accounting.Application.Features.ClientRequests.Reject;
using Accounting.Application.Features.ClientRequests.UnassignClientRequestUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
namespace Accounting.Api.Controllers
{
    [ApiController]
    [Route("api/client-requests")]
    public class ClientRequestController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ClientRequestController> _logger;

        public ClientRequestController(IMediator mediator, ILogger<ClientRequestController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(IReadOnlyList<ClientRequestDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<ClientRequestDto>>> List(CancellationToken ct)
        {
            _logger.LogDebug("Listing client requests");
            var clientRequests = await _mediator.Send(new ListClientRequestsQuery(), ct);
            return Ok(clientRequests);
        }

        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ClientRequestDto>> GetById(Guid id, CancellationToken ct)
        {
            _logger.LogDebug("Getting client request by id: {ClientRequestId}", id);
            var clientRequest = await _mediator.Send(new GetClientRequestQuery(id), ct);

            return Ok(clientRequest);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateClientRequestRequest request, CancellationToken ct) 
        {
            var clientRequestId = await _mediator.Send(new CreateClientRequestCommand(request.FullName, request.Email, request.PhoneNumber, request.Message, request.ServiceId, request.PricingPackageId, request.RequestType), ct);
            return CreatedAtAction(nameof(GetById), new { id = clientRequestId }, new { id = clientRequestId });
        }

        [HttpPatch("{id:guid}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ChangeStatus(Guid id, [FromBody] ChangeClientRequestStatusRequest request, CancellationToken ct) 
        {
            await _mediator.Send(new ChangeClientRequestStatusCommand(id, request.Status), ct);
            return NoContent();
        }

        [HttpPatch("{id:guid}/assign-user")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AssignUser(
    Guid id,
    [FromBody] AssignClientRequestToUserRequest request,
    CancellationToken ct)
        {
            await _mediator.Send(
                new AssignClientRequestToUserCommand(id, request.UserId),
                ct);

            return NoContent();
        }

        [HttpPatch("{id:guid}/unassign-user")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UnassignUser(
            Guid id,
            CancellationToken ct)
        {
            await _mediator.Send(
                new UnassignClientRequestUserCommand(id),
                ct);

            return NoContent();
        }

        [HttpPatch("{id:guid}/admin-note")]
        [Authorize(Roles= "Admin")]
        public async Task<ActionResult> ChangeAdminNote(Guid id, ChangeAdminNoteRequest request, CancellationToken ct) 
        {
            await _mediator.Send(new ChangeAdminNoteCommand(id, request.adminNote), ct);
            return NoContent();
        }

        [HttpPatch("{id:guid}/complete")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> CompleteClientRequest(Guid id, CancellationToken ct)
        {
            await _mediator.Send(new MarkCompletedCommand(id), ct);
            return NoContent();
        }

        [HttpPatch("{id:guid}/reject")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> RejectClientRequest(Guid id, CancellationToken ct)
        {
            await _mediator.Send(new MarkRejectedCommand(id), ct);
            return NoContent();
        }
    }
}
