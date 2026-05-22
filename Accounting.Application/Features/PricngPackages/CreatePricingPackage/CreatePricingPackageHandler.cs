using Accounting.Application.Abstractions.Persistence;
using Accounting.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.PricngPackages.CreatePricingPackage
{
    internal class CreatePricingPackageHandler : IRequestHandler<CreatePricingPackageCommand, Guid>
    {
        private readonly IPricingPackageRepository _pricingPackageRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreatePricingPackageHandler(IPricingPackageRepository pricingPackageRepository, IUnitOfWork unitOfWork)
        {
            _pricingPackageRepository = pricingPackageRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Guid> Handle(CreatePricingPackageCommand request, CancellationToken cancellationToken)
        {
           var pricingPackage = PricingPackage.Create(
                request.Name,
                request.Price,
                request.PriceLabel,
                request.PeriodLabel,
                request.Badge,
                request.Description,
                request.IsRecommended,
                request.SortOrder
            );

            pricingPackage.ReplaceFeatures(request.Features);

            await _pricingPackageRepository.AddAsync(pricingPackage, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return pricingPackage.Id;
        }
    }
}
