using Accounting.Application.Abstractions.Persistence;
using Accounting.Application.Features.ClientRequests.Common;
using Accounting.Application.Features.Services.Common;
using Accounting.Domain.Entities;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Services.Get
{
    public sealed class GetServiceByIdHandler : IRequestHandler<GetServiceQuery, ServiceDto>
    {
        public readonly IServiceRepository _serviceRepository;
        public readonly IMapper _mapper;

        public GetServiceByIdHandler(IServiceRepository serviceRepository, IMapper mapper)
        {
            _serviceRepository = serviceRepository ?? throw new ArgumentNullException(nameof(serviceRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ServiceDto> Handle(GetServiceQuery request, CancellationToken cancellationToken)
        {
            var service = await _serviceRepository.GetByIdAsync(request.Id, cancellationToken);

            if (service is null)
            {
                throw new Application.Common.Errors.NotFoundException($"Service with id {request.Id} not found.");
            }

            return _mapper.Map<ServiceDto>(service);
        }
    }
}
