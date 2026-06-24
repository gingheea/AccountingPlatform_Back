using Accounting.Application.Features.ClientRequests.Common;
using Accounting.Application.Features.Portal.Common;
using Accounting.Application.Features.Portal.GetCurrentUser;
using Accounting.Application.Features.Portal.ListClientRequests;
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
        private readonly IMediator _mediator;
        private readonly ILogger<UserPortalController> _logger;

        public UserPortalController(IMediator mediator, ILogger<UserPortalController> logger)
        {
            _mediator = mediator;
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
    }
}
