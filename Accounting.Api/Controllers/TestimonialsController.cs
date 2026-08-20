using Accounting.Api.Contracts.Testimonials;
using Accounting.Application.Features.Testimonials.Common;
using Accounting.Application.Features.Testimonials.ListForAdmin;
using Accounting.Application.Features.Testimonials.ListPublished;
using Accounting.Application.Features.Testimonials.Moderate;
using Accounting.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.Api.Controllers
{
    [ApiController]
    [Route("api/testimonials")]
    public class TestimonialsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TestimonialsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>Схвалені відгуки для публічних сторінок.</summary>
        [HttpGet("published")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IReadOnlyList<PublicTestimonialDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<PublicTestimonialDto>>> Published(
            [FromQuery] int take = 6,
            CancellationToken ct = default)
        {
            var testimonials = await _mediator.Send(new ListPublishedTestimonialsQuery(take), ct);

            return Ok(testimonials);
        }

        /// <summary>Усі відгуки з їхнім станом — для розгляду в адмінці.</summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(IReadOnlyList<TestimonialDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<TestimonialDto>>> List(
            [FromQuery] TestimonialStatus? status,
            CancellationToken ct)
        {
            var testimonials = await _mediator.Send(new ListTestimonialsQuery(status), ct);

            return Ok(testimonials);
        }

        [HttpPost("{id:guid}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Approve(Guid id, CancellationToken ct)
        {
            await _mediator.Send(new ApproveTestimonialCommand(id), ct);

            return NoContent();
        }

        [HttpPost("{id:guid}/reject")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Reject(
            Guid id,
            [FromBody] RejectTestimonialRequest? request,
            CancellationToken ct)
        {
            await _mediator.Send(new RejectTestimonialCommand(id, request?.Note), ct);

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(Guid id, CancellationToken ct)
        {
            await _mediator.Send(new DeleteTestimonialCommand(id), ct);

            return NoContent();
        }
    }
}
