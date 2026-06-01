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

namespace Accounting.Application.Features.Services.List
{
    public sealed class GetServicesHandler : IRequestHandler<GetListServicesQuery, IReadOnlyList<ServiceDto>>
    {
        public readonly IServiceRepository _serviceRepository;
        public readonly IMapper _mapper;

        public GetServicesHandler(IServiceRepository serviceRepository, IMapper mapper)
        {
            _serviceRepository = serviceRepository ?? throw new ArgumentNullException(nameof(serviceRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IReadOnlyList<ServiceDto>> Handle(GetListServicesQuery request, CancellationToken cancellationToken)
        {
            return await _serviceRepository.Query()
           .AsNoTracking()
           .OrderBy(x => x.SortOrder)
           .ThenBy(x => x.Name)
           .ProjectTo<ServiceDto>(_mapper.ConfigurationProvider)
           .ToListAsync(cancellationToken);
        }
    }
}
