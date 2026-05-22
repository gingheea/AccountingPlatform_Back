using Accounting.Application.Abstractions.Persistence;
using Accounting.Domain.Entities;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.PricngPackages.DeletePricingPackage
{
    public class DeletePricingPackageHandler : IRequestHandler<DeletePricingPackageRequest>
    {
        private readonly IPricingPackageRepository _pricingPackageRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeletePricingPackageHandler(IPricingPackageRepository pricingPackageRepository, IUnitOfWork unitOfWork)
        {
            _pricingPackageRepository = pricingPackageRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeletePricingPackageRequest request, CancellationToken cancellationToken)
        {
            var pricingPackage = await _pricingPackageRepository.GetByIdAsync(request.Id, cancellationToken);
            if (pricingPackage == null) 
            {
                throw new InvalidOperationException("Pricing package not found");
            }

            _pricingPackageRepository.Remove(pricingPackage);

        }
    }
}
