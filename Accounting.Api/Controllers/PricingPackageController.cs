using Accounting.Api.Contracts.PricingPackagaes;
using Accounting.Api.Contracts.Services;
using Accounting.Application.Features.PricingPackages.Common;
using Accounting.Application.Features.PricngPackages.ActivatePricingPackage;
using Accounting.Application.Features.PricngPackages.CreatePricingPackage;
using Accounting.Application.Features.PricngPackages.DeactivatePricingPackage;
using Accounting.Application.Features.PricngPackages.DeletePricingPackage;
using Accounting.Application.Features.PricngPackages.GetPricingPackageById;
using Accounting.Application.Features.PricngPackages.GetPricingPackages;
using Accounting.Application.Features.PricngPackages.UpdatePricingPackage;
using Accounting.Application.Features.Services.ActivateService;
using Accounting.Application.Features.Services.CreateService;
using Accounting.Application.Features.Services.DeactivateService;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.Api.Controllers
{
    [ApiController]
    [Route("api/pricing-packages")]
    public class PricingPackageController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PricingPackageController> _logger;

        public PricingPackageController(IMediator mediator, ILogger<PricingPackageController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("{id:guid}", Name = "GetPricingPackageById")]
        [AllowAnonymous]
        public async Task<ActionResult<PricingPackageDto>> GetPricingPackageById(Guid id, CancellationToken ct)
        {
            var pricingPackage = await _mediator.Send(new GetPricingPackageQuery(id), ct);
            return Ok(pricingPackage);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IReadOnlyList<PricingPackageDto>>> GetPricingPackages(CancellationToken ct)
        {
            var pricingPackages = await _mediator.Send(new GetPricingPackagesQuery(), ct);
            return Ok(pricingPackages);
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> CreatePricingPackage([FromBody] CreatePricingPackageRequest request, CancellationToken ct)
        {
            var id = await _mediator.Send(
                new CreatePricingPackageCommand(
                    request.Name,
                    request.Badge,
                    request.Description,
                    request.Price,
                    request.PriceLabel,
                    request.PeriodLabel,
                    request.IsRecommended ?? false,
                    request.Features ?? Array.Empty<string>(),
                    request.SortOrder ?? 0
                ),
                ct);

            return Ok(id);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdatePricingPackage(Guid id, [FromBody] UpdatePricingPackageRequest request, CancellationToken ct)
        {
            await _mediator.Send(
                new UpdatePricingPackageCommand(
                    id,
                    request.Name,
                    request.Badge,
                    request.Description,
                    request.Price,
                    request.PriceLabel,
                    request.PeriodLabel,
                    request.IsRecommended ?? false,
                    request.IsActive ?? true,
                    request.Features ?? Array.Empty<string>(),
                    request.SortOrder ?? 0
                ),
                ct);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeletePricingPackage(Guid id, CancellationToken ct)
        {
            await _mediator.Send(new DeletePricingPackageCommand(id), ct);
            return NoContent();
        }

        [HttpPatch("{id:guid}/deactivate")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Deactivate(Guid id, CancellationToken ct)
        {
            await _mediator.Send(new DeactivatePricingPackageCommand(id), ct);
            return NoContent();
        }

        [HttpPatch("{id:guid}/activate")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Activate(Guid id, CancellationToken ct)
        {
            await _mediator.Send(new ActivatePricingPackageCommand(id), ct);
            return NoContent();
        }
    }
}
