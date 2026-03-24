using Accounting.Application.Abstractions.Persistence;
using Accounting.Application.Common.Errors;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Services.DeactivateService
{
    internal class DeactivateServiceHandler : IRequestHandler<DeactivateCommand>
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeactivateServiceHandler(IServiceRepository serviceRepository, IUnitOfWork unitOfWork)
        {
            _serviceRepository = serviceRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeactivateCommand request, CancellationToken ct)
        {
           var service = await _serviceRepository.GetByIdAsync(request.id, ct);

            if (service is null)
            {
                throw new NotFoundException("Service not found");
            }

            service.Deactivate();

            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}
