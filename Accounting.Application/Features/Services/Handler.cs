using Accounting.Application.Abstractions.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Services
{
    public sealed class Handler : IRequestHandler<Query, IReadOnlyList<ServiceDto>>
    {
        public readonly IServiceRepository _serviceRepository;

        public Handler(IServiceRepository serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }

        public async Task<IReadOnlyList<ServiceDto>> Handle(Query request, CancellationToken cancellationToken)
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
