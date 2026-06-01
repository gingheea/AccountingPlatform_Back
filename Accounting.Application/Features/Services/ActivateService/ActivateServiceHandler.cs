using Accounting.Application.Abstractions.Persistence;
using Accounting.Application.Common.Errors;
using Accounting.Application.Features.Services.DeactivateService;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Services.ActivateService
{
    internal class ActivateServiceHandler : IRequestHandler<ActivateCommand>
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ActivateServiceHandler(IServiceRepository serviceRepository, IUnitOfWork unitOfWork)
        {
            _serviceRepository = serviceRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(ActivateCommand request, CancellationToken ct)
        {
            var service = await _serviceRepository.GetByIdAsync(request.id, ct);

            if (service is null)
            {
                throw new NotFoundException("Service not found");
            }

            service.Activate();

            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}
