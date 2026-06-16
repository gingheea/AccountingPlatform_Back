using Accounting.Api.Contracts.Identity;
using Accounting.Application.Features.Users.ActivateUser;
using Accounting.Application.Features.Users.ChangeRolesForUser;
using Accounting.Application.Features.Users.Common;
using Accounting.Application.Features.Users.CreateUser;
using Accounting.Application.Features.Users.DeactivateUser;
using Accounting.Application.Features.Users.GetUser;
using Accounting.Application.Features.Users.GetUsers;
using Accounting.Application.Features.Users.ResetUserPassword;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.Api.Controllers.Identity
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IMediator mediator, ILogger<UsersController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IReadOnlyList<UserDto>>> List(CancellationToken ct)
        {
            _logger.LogDebug("Listing users");
            var users = await _mediator.Send(new GetUsersQuery(), ct);
            return Ok(users);
        }

        [HttpGet("{id:guid}", Name = "GetUserById")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDto>> GetUserById(Guid id, CancellationToken ct)
        {
            _logger.LogDebug("Getting user with id {id}", id);
            var user = await _mediator.Send(new GetUserByIdQuery(id), ct);
            return Ok(user);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> CreateUser(Contracts.Identity.CreateUserRequest request, CancellationToken ct)
        {
           var userId = await _mediator.Send(new Application.Features.Users.CreateUser.CreateUserRequest(request.FullName, request.Email, request.Password, request.TaxId, request.Roles), ct);
            return CreatedAtRoute("GetUserById", new { id = userId }, null);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateUser(Guid id, UpdateUserRequest request, CancellationToken ct)
        {
            _logger.LogDebug("Updating user with id {id}", id);
            await _mediator.Send(new Application.Features.Users.UpdateUser.UpdateUserRequest(id, request.FullName, request.Email, request.TaxId, request.IsActive), ct);
            return NoContent();
        }

        [HttpPatch("{id:guid}/activate")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ActivateUser(Guid id, CancellationToken ct)
        {
            _logger.LogDebug("Activating user with id {id}", id);
            await _mediator.Send(new ActivateUserRequest(id), ct);
            return NoContent();
        }

        [HttpPatch("{id:guid}/deactivate")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeactivateUser(Guid id, CancellationToken ct)
        {
            _logger.LogDebug("Deactivating user with id {id}", id);
            await _mediator.Send(new DeactivateUserRequest(id), ct);
            return NoContent();
        }

        [HttpPatch("{id:guid}/roles")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateUserRoles(Guid id, Contracts.Identity.ChangeUserRolesRequest request, CancellationToken ct)
        {
            _logger.LogDebug("Updating user roles with id {id}", id);
            await _mediator.Send(new Application.Features.Users.ChangeRolesForUser.ChangeUserRolesRequest(id, request.Roles), ct);
            return NoContent();
        }

        [HttpPatch("{id:guid}/reset-password")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateUserPassword(Guid id, ResetPasswordRequest request, CancellationToken ct)
        {
            _logger.LogDebug("Updating user password with id {id}", id);
            await _mediator.Send(new ResetUserPasswordRequest(id, request.NewPassword), ct);
            return NoContent();
        }
    }
}
