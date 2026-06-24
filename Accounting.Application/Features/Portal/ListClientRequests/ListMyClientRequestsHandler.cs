using Accounting.Application.Abstractions.Identity;
using Accounting.Application.Abstractions.Persistence;
using Accounting.Application.Features.ClientRequests.Common;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Portal.ListClientRequests
{

    public sealed class ListMyClientRequestsHandler
     : IRequestHandler<ListMyClientRequestsQuery, IReadOnlyList<ClientRequestDto>>
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

        public async Task<IReadOnlyList<ClientRequestDto>> Handle(
            ListMyClientRequestsQuery request,
            CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            if (userId is null)
                throw new UnauthorizedAccessException("Current user is not authenticated.");

            return await _repository
                .Query()
                .AsNoTracking()
                .Where(x => x.UserId == userId.Value)
                .OrderByDescending(x => x.CreatedAtUtc)
                .ProjectTo<ClientRequestDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
        }
    }
}
