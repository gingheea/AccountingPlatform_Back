using Accounting.Application.Abstractions.Persistence;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Services.DeleteService
{
    internal class DeleteServiceHandler : IRequestHandler<DeleteServiceCommand>
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteServiceHandler(IServiceRepository serviceRepository, IUnitOfWork unitOfWork)
        {
            _serviceRepository = serviceRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteServiceCommand request, CancellationToken cancellationToken)
        {
            var service = await _serviceRepository.GetByIdAsync(request.Id, cancellationToken);
            if (service == null) 
            {
                throw new InvalidOperationException("Service not found");
            }

            _serviceRepository.Remove(service);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
