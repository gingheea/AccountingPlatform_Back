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

namespace Accounting.Application.Features.ClientRequests.ListClientRequests
{
    public class ListClientRequestsHandler
        : IRequestHandler<ListClientRequestsQuery, PagedResult<ClientRequestDto>>
    {
        private readonly IClientRequestRepository _repository;
        private readonly IMapper _mapper;

        public ListClientRequestsHandler(IClientRequestRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<PagedResult<ClientRequestDto>> Handle(
            ListClientRequestsQuery request,
            CancellationToken ct)
        {
            return await _repository
                .Query()
                .AsNoTracking()
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
