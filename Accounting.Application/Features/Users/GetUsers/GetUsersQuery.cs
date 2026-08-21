using Accounting.Application.Common;
using Accounting.Application.Features.Users.Common;
using MediatR;

namespace Accounting.Application.Features.Users.GetUsers
{
    /// <param name="Search">
    /// Шукає по імені, пошті й податковому коду. Фільтр мусить бути на сервері:
    /// щойно список порізаний на сторінки, пошук у браузері шукав би лише
    /// по тому, що вже завантажено, — і мовчки не знаходив би решту.
    /// </param>
    public sealed record GetUsersQuery(
        string? Search,
        bool? IsActive,
        int Page,
        int PageSize
    ) : IRequest<PagedResult<UserDto>>;
}
