using Accounting.Application.Abstractions.Persistence;
using Accounting.Application.Common.Errors;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.PricngPackages.DeactivatePricingPackage
{
    internal class DeactivatePricingPackageHandler : IRequestHandler<DeactivatePricingPackageCommand>
    {
        private readonly IPricingPackageRepository _pricingPackageRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeactivatePricingPackageHandler(IPricingPackageRepository pricingPackageRepository, IUnitOfWork unitOfWork)
        {
            _pricingPackageRepository = pricingPackageRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeactivatePricingPackageCommand request, CancellationToken cancellationToken)
        {
            var pricingPackage = await _pricingPackageRepository.GetByIdAsync(request.Id, cancellationToken);

            if (pricingPackage is null)
            {
                throw new NotFoundException("Pricing package not found");
            }

            pricingPackage.Deactivate();

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
