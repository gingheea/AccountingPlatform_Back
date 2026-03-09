using Accounting.Application.Abstractions.Persistence;
using Accounting.Application.Features.Services.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Services.List
{
    public sealed class Handler : IRequestHandler<GetListServicesQuery, IReadOnlyList<ServiceDto>>
    {
        public readonly IServiceRepository _serviceRepository;

        public Handler(IServiceRepository serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }

        public async Task<IReadOnlyList<ServiceDto>> Handle(GetListServicesQuery request, CancellationToken cancellationToken)
        {
            return await _serviceRepository.Query()
           .AsNoTracking()
           .Where(x => x.IsActive)
           .OrderBy(x => x.SortOrder)
           .ThenBy(x => x.Name)
           .Select(x => new ServiceDto(x.Id, x.Name, x.Description, x.Price, x.IsActive, x.SortOrder ))
           .ToListAsync(cancellationToken);
        }
    }
}
