using Accounting.Application.Abstractions.Identity;
using System.Security.Claims;

namespace Accounting.Api.Service
{
    public sealed class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? UserId
        {
            get
            {
                var user = _httpContextAccessor.HttpContext?.User;

                var value =
                    user?.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                    user?.FindFirst("sub")?.Value ??
                    user?.FindFirst("nameid")?.Value;

                return Guid.TryParse(value, out var userId)
                    ? userId
                    : null;
            }
        }

        public string? Email
        {
            get
            {
                var user = _httpContextAccessor.HttpContext?.User;

                return user?.FindFirst(ClaimTypes.Email)?.Value ??
                       user?.FindFirst("email")?.Value;
            }
        }

        public bool IsAuthenticated =>
            _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true;
    }
}
