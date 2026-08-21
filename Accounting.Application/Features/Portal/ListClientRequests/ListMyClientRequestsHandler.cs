using Accounting.Application.Abstractions.Identity;
using Accounting.Application.Abstractions.Persistence;
using Accounting.Application.Common;
using Accounting.Application.Features.ClientRequests.Common;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Portal.ListClientRequests
{
    public sealed class ListMyClientRequestsHandler
        : IRequestHandler<ListMyClientRequestsQuery, PagedResult<ClientRequestDto>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IClientRequestRepository _repository;
        private readonly IMapper _mapper;

        public ListMyClientRequestsHandler(
            ICurrentUserService currentUserService,
            IClientRequestRepository repository,
            IMapper mapper)
        {
            _currentUserService = currentUserService;
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedResult<ClientRequestDto>> Handle(
            ListMyClientRequestsQuery request,
            CancellationToken ct)
        {
            var userId = _currentUserService.UserId;

            if (userId is null)
                throw new UnauthorizedAccessException("Current user is not authenticated.");

            return await _repository
                .Query()
                .AsNoTracking()
                // The owner filter comes before paging: otherwise a client would see other
                // people's requests simply by asking for a different page.
                .Where(x => x.UserId == userId.Value)
                .OrderByDescending(x => x.CreatedAtUtc)
                .ThenBy(x => x.Id)
                .ToPagedResultAsync(
                    q => q.ProjectTo<ClientRequestDto>(_mapper.ConfigurationProvider),
                    request.Page,
                    request.PageSize,
                    ct);
        }
    }
}
