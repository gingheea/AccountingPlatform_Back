using Accounting.Api.Contracts.ClientRequests;
using Accounting.Application.Features.Services.Common;
using Accounting.Application.Features.Services.CreateService;
using Accounting.Application.Features.Services.Get;
using Accounting.Application.Features.Services.List;
using Accounting.Application.Features.Services.UpdateService;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Accounting.Api.Controllers;

[ApiController]
[Route("api/services")]
public sealed class ServicesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ServicesController> _logger;

    public ServicesController(IMediator mediator, ILogger<ServicesController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IReadOnlyList<ServiceDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ServiceDto>>> List(CancellationToken ct)
    {
        _logger.LogDebug("Listing services");
        var services = await _mediator.Send(new GetListServicesQuery(), ct);
        return Ok(services);
    }

    [HttpGet("{id:guid}", Name = "GetServiceById")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ServiceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceDto>> GetById(Guid id, CancellationToken ct)
    {
        _logger.LogDebug("Getting service by id: {ServiceId}", id);
        var service = await _mediator.Send(new GetServiceQuery(id), ct);

        if (service is null)
        {
            _logger.LogInformation("Service not found: {ServiceId}", id);
            return NotFound();
        }

        return Ok(service);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<Guid>> Create([FromBody]CreateServiceRequest request, CancellationToken ct)
    {
        if (request is null)
        {
            _logger.LogWarning("Create request was null");
            return BadRequest();
        }

        _logger.LogDebug("Creating service: {Name}", request.Name);

        var id = await _mediator.Send(
            new CreateServiceCommand(
                request.Name,
                request.Description,
                request.Price,
                request.SortOrder
            ),
            ct);

        _logger.LogInformation("Service created with id: {ServiceId}", id);

        return CreatedAtRoute("GetServiceById", new { id }, new { id });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody]UpdateServiceRequest request, CancellationToken ct)
    {
        if (request is null)
        {
            _logger.LogWarning("Update request was null");
            return BadRequest();
        }

        _logger.LogDebug("Updating service with id: {ServiceId}", id);
        await _mediator.Send(
            new UpdateServiceCommand(
                id,
                request.Name,
                request.Description,
                request.Price,
                request.IsActive,
                request.SortOrder
            ),
            ct);

        _logger.LogInformation("Service updated with id: {ServiceId}", id);
        return NoContent();
    }
}