using Accounting.Application.Common;
using Accounting.Api.Contracts.ClientDocuments;
using Accounting.Application.Features.ClientDocuments.ChangeStatus;
using Accounting.Application.Features.ClientDocuments.Common;
using Accounting.Application.Features.ClientDocuments.DeleteDocument;
using Accounting.Application.Features.ClientDocuments.GetDownloadUrl;
using Accounting.Application.Features.ClientDocuments.ListDocuments;
using Accounting.Application.Features.ClientDocuments.UploadDocument;
using Accounting.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.Api.Controllers
{
    [ApiController]
    [Route("api/client-documents")]
    [Authorize(Roles = "Admin")]
    public class ClientDocumentsController : ControllerBase
    {
        private const long MaxUploadBytes = 21 * 1024 * 1024;

        private readonly IMediator _mediator;
        private readonly ILogger<ClientDocumentsController> _logger;

        public ClientDocumentsController(IMediator mediator, ILogger<ClientDocumentsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<ClientDocumentDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<ClientDocumentDto>>> List(
            [FromQuery] Guid? userId,
            [FromQuery] ClientDocumentCategory? category,
            [FromQuery] ClientDocumentDirection? direction,
            [FromQuery] ClientDocumentStatus? status,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = Pagination.DefaultPageSize,
            CancellationToken ct = default)
        {
            var documents = await _mediator.Send(
                new ListDocumentsQuery(userId, category, direction, status, page, pageSize), ct);

            return Ok(documents);
        }

        [HttpPost]
        [RequestSizeLimit(MaxUploadBytes)]
        public async Task<ActionResult<Guid>> Upload(
            [FromForm] UploadDocumentRequest request,
            CancellationToken ct)
        {
            if (request.File is null || request.File.Length == 0)
                return BadRequest(new { type = "validation_error", title = "File is required.", status = 400 });

            _logger.LogInformation(
                "Uploading document for user {UserId}: {FileName}",
                request.UserId,
                request.File.FileName);

            await using var content = request.File.OpenReadStream();

            var documentId = await _mediator.Send(
                new UploadDocumentCommand(
                    request.UserId,
                    request.Title,
                    request.File.FileName,
                    request.File.ContentType,
                    request.File.Length,
                    content,
                    request.Category,
                    request.Direction,
                    request.Note),
                ct);

            return Ok(new { id = documentId });
        }

        [HttpGet("{id:guid}/download-url")]
        [ProducesResponseType(typeof(DocumentDownloadDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<DocumentDownloadDto>> GetDownloadUrl(Guid id, CancellationToken ct)
        {
            var download = await _mediator.Send(new GetDocumentDownloadUrlQuery(id, null), ct);

            return Ok(download);
        }

        [HttpPatch("{id:guid}/status")]
        public async Task<ActionResult> ChangeStatus(
            Guid id,
            [FromBody] ChangeDocumentStatusRequest request,
            CancellationToken ct)
        {
            await _mediator.Send(new ChangeDocumentStatusCommand(id, request.Status, request.Note), ct);

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> Delete(Guid id, CancellationToken ct)
        {
            await _mediator.Send(new DeleteDocumentCommand(id), ct);

            return NoContent();
        }
    }
}
