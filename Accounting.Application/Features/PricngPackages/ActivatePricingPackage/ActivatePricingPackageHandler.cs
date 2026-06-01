using Accounting.Application.Abstractions.Persistence;
using Accounting.Application.Common.Errors;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.PricngPackages.ActivatePricingPackage
{
    internal class ActivatePricingPackageHandler : IRequestHandler<ActivatePricingPackageCommand>
    {
        private readonly IPricingPackageRepository _pricingPackageRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ActivatePricingPackageHandler(IPricingPackageRepository pricingPackageRepository, IUnitOfWork unitOfWork)
        {
            _pricingPackageRepository = pricingPackageRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(ActivatePricingPackageCommand request, CancellationToken cancellationToken)
        {
            var pricingPackage = await _pricingPackageRepository.GetByIdAsync(request.Id, cancellationToken);

            if (pricingPackage is null)
            {
                throw new NotFoundException("Pricing package not found");
            }

            pricingPackage.Activate();

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
