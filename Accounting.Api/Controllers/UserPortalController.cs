using Accounting.Api.Contracts.ClientDocuments;
using Accounting.Application.Abstractions.Identity;
using Accounting.Application.Features.ClientDocuments.Common;
using Accounting.Application.Features.ClientDocuments.GetDownloadUrl;
using Accounting.Application.Features.ClientDocuments.ListDocuments;
using Accounting.Application.Features.ClientDocuments.UploadDocument;
using Accounting.Application.Features.ClientRequests.Common;
using Accounting.Application.Features.ClientSubscriptions.Common;
using Accounting.Application.Features.ClientSubscriptions.ListSubscriptions;
using Accounting.Application.Features.Portal.Common;
using Accounting.Application.Features.Portal.GetCurrentUser;
using Accounting.Application.Features.Portal.ListClientRequests;
using Accounting.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.Api.Controllers
{
    [ApiController]
    [Route("api/portal")]
    [Authorize(Roles = "User,Admin")]
    public class UserPortalController : ControllerBase
    {
        private const long MaxUploadBytes = 21 * 1024 * 1024;

        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<UserPortalController> _logger;

        public UserPortalController(
            IMediator mediator,
            ICurrentUserService currentUserService,
            ILogger<UserPortalController> logger)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        [HttpGet("me")]
        public async Task<ActionResult<PortalUserDto>> Me(CancellationToken ct)
        {
            _logger.LogDebug("Getting current portal user");

            var user = await _mediator.Send(new GetCurrentUserQuery(), ct);

            return Ok(user);
        }

        [HttpGet("client-requests")]
        public async Task<ActionResult<IReadOnlyList<ClientRequestDto>>> MyClientRequests(
      CancellationToken ct)
        {
            var requests = await _mediator.Send(new ListMyClientRequestsQuery(), ct);

            return Ok(requests);
        }

        [HttpGet("subscriptions")]
        [ProducesResponseType(typeof(IReadOnlyList<ClientSubscriptionDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<ClientSubscriptionDto>>> MySubscriptions(
            CancellationToken ct)
        {
            // Id підставляємо з токена, а не з запиту — інакше клієнт міг би
            // попросити чужі дані, просто змінивши параметр.
            var subscriptions = await _mediator.Send(
                new ListClientSubscriptionsQuery(CurrentUserId(), Status: null), ct);

            return Ok(subscriptions);
        }

        [HttpGet("documents")]
        [ProducesResponseType(typeof(IReadOnlyList<ClientDocumentDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<ClientDocumentDto>>> MyDocuments(
            [FromQuery] ClientDocumentCategory? category,
            [FromQuery] ClientDocumentDirection? direction,
            [FromQuery] ClientDocumentStatus? status,
            CancellationToken ct)
        {
            var documents = await _mediator.Send(
                new ListDocumentsQuery(CurrentUserId(), category, direction, status), ct);

            return Ok(documents);
        }

        [HttpPost("documents")]
        [RequestSizeLimit(MaxUploadBytes)]
        public async Task<ActionResult<Guid>> UploadMyDocument(
            [FromForm] UploadMyDocumentRequest request,
            CancellationToken ct)
        {
            if (request.File is null || request.File.Length == 0)
                return BadRequest(new { type = "validation_error", title = "File is required.", status = 400 });

            var userId = CurrentUserId();

            _logger.LogInformation(
                "Client {UserId} is uploading a document: {FileName}",
                userId,
                request.File.FileName);

            await using var content = request.File.OpenReadStream();

            var documentId = await _mediator.Send(
                new UploadDocumentCommand(
                    userId,
                    request.Title,
                    request.File.FileName,
                    request.File.ContentType,
                    request.File.Length,
                    content,
                    request.Category,
                    // A client can only ever hand files to the accountant, never the other way round.
                    ClientDocumentDirection.ClientToAccountant,
                    request.Note),
                ct);

            return Ok(new { id = documentId });
        }

        [HttpGet("documents/{id:guid}/download-url")]
        [ProducesResponseType(typeof(DocumentDownloadDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<DocumentDownloadDto>> GetMyDocumentDownloadUrl(
            Guid id,
            CancellationToken ct)
        {
            var download = await _mediator.Send(
                new GetDocumentDownloadUrlQuery(id, CurrentUserId()), ct);

            return Ok(download);
        }

        private Guid CurrentUserId()
            => _currentUserService.UserId
               ?? throw new UnauthorizedAccessException("Current user is not authenticated.");
    }
}
