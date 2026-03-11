using Accounting.Application.Abstractions.Persistence;
using Accounting.Application.Features.Services.CreateService;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Services.UpdateService
{
    public sealed class UpdateServiceHandler : IRequestHandler<UpdateServiceCommand>
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateServiceHandler(IServiceRepository serviceRepository, IUnitOfWork unitOfWork)
        {
            _serviceRepository = serviceRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task Handle(UpdateServiceCommand request, CancellationToken ct)
        {
            var service = await _serviceRepository.GetByIdAsync(request.Id, ct);

            if (service is null)
            {
                throw new Application.Common.Errors.NotFoundException($"Service with id {request.Id} not found.");
            }

            service.Update(request.Name, request.Price, request.Description, request.IsActive, request.SortOrder);
            await _serviceRepository.UpdateAsync(service, ct);
            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}
