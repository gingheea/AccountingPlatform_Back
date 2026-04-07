using Accounting.Application.Abstractions.Persistence;
using Accounting.Application.Features.ClientRequests.Common;
using Accounting.Application.Features.Services.Common;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.ClientRequests.ListClientRequests
{
    public class ListClientRequestsHandler : IRequestHandler<ListClientRequestsQuery, IReadOnlyList<ClientRequestDto>>
    {
        private readonly IClientRequestRepository _repository;
        private readonly IMapper _mapper;

        public ListClientRequestsHandler(IClientRequestRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IReadOnlyList<ClientRequestDto>> Handle(ListClientRequestsQuery request, CancellationToken cancellationToken)
        {
            return await _repository
                .Query()
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAtUtc)
                .ProjectTo<ClientRequestDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
        }
    }
}
