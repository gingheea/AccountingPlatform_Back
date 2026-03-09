using Accounting.Api.Contracts.ClientRequests;
using Accounting.Api.Contracts.Services;
using Accounting.Application.Features.Services.Common;
using Accounting.Application.Features.Services.CreateService;
using Accounting.Application.Features.Services.Get;
using Accounting.Application.Features.Services.List;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.Api.Controllers;

[ApiController]
[Route("api/services")]
public sealed class ServicesController : ControllerBase
{
    private readonly IMediator _mediator;
    public ServicesController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<ServiceDto>>> List(CancellationToken ct)
        => Ok(await _mediator.Send(new GetListServicesQuery(), ct));

    [HttpGet]
    [Route("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<ServiceDto>> GetById(Guid id, CancellationToken ct)
    {
        var service = await _mediator.Send(new GetServiceQuery(id), ct);

        if (service is null)
            return NotFound();

        return Ok(service);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Guid>> Create(CreateServiceRequest request, CancellationToken ct)
    {
        var id = await _mediator.Send(
            new CreateServiceCommand(
                request.Name,
                request.Description,
                request.Price,
                request.SortOrder
            ),
            ct);

        return CreatedAtAction(nameof(GetById), new { id }, null);
    }
}