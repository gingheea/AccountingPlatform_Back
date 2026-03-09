using Accounting.Application.Abstractions.Persistence;
using Accounting.Application.Features.Services.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Services.Get
{
    public sealed class Handler : IRequestHandler<GetServiceQuery, ServiceDto>
    {
        public readonly IServiceRepository _serviceRepository;

        public Handler(IServiceRepository serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }

        public async Task<ServiceDto> Handle(GetServiceQuery request, CancellationToken cancellationToken)
        {
            var service = await _serviceRepository.GetByIdAsync(request.Id, cancellationToken);

            if (service is null)
            {
                throw new Application.Common.Errors.NotFoundException($"Service with id {request.Id} not found.");
            }

            return new ServiceDto(service.Id, service.Name, service.Description, service.Price, service.IsActive, service.SortOrder);
        }
    }
}
