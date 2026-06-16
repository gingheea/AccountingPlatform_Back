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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PricingPackageDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PricingPackageDto>> GetPricingPackageById(Guid id, CancellationToken ct)
        {
            _logger.LogInformation("GetPricingPackageById called with id={PricingPackageId}", id);
            var pricingPackage = await _mediator.Send(new GetPricingPackageQuery(id), ct);
            _logger.LogInformation("GetPricingPackageById completed for id={PricingPackageId}", id);
            return Ok(pricingPackage);
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyList<PricingPackageDto>))]
        public async Task<ActionResult<IReadOnlyList<PricingPackageDto>>> GetPricingPackages(CancellationToken ct)
        {
            _logger.LogInformation("GetPricingPackages called");
            var pricingPackages = await _mediator.Send(new GetPricingPackagesQuery(), ct);
            _logger.LogInformation("GetPricingPackages returned {Count} items", pricingPackages?.Count ?? 0);
            return Ok(pricingPackages);
        }


        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreatePricingPackage([FromBody] CreatePricingPackageRequest request, CancellationToken ct)
        {
            _logger.LogInformation("CreatePricingPackage called with Name={Name}", request?.Name);
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

            _logger.LogInformation("CreatePricingPackage succeeded with id={PricingPackageId}", id);

            return Ok(id);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdatePricingPackage(Guid id, [FromBody] UpdatePricingPackageRequest request, CancellationToken ct)
        {
            _logger.LogInformation("UpdatePricingPackage called for id={PricingPackageId}", id);
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

            _logger.LogInformation("UpdatePricingPackage completed for id={PricingPackageId}", id);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeletePricingPackage(Guid id, CancellationToken ct)
        {
            _logger.LogInformation("DeletePricingPackage called for id={PricingPackageId}", id);
            await _mediator.Send(new DeletePricingPackageCommand(id), ct);

            _logger.LogInformation("DeletePricingPackage completed for id={PricingPackageId}", id);
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
            _logger.LogInformation("Deactivate called for id={PricingPackageId}", id);
            await _mediator.Send(new DeactivatePricingPackageCommand(id), ct);
            _logger.LogInformation("Deactivate completed for id={PricingPackageId}", id);
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
            _logger.LogInformation("Activate called for id={PricingPackageId}", id);
            await _mediator.Send(new ActivatePricingPackageCommand(id), ct);
            _logger.LogInformation("Activate completed for id={PricingPackageId}", id);
            return NoContent();
        }
    }
}
