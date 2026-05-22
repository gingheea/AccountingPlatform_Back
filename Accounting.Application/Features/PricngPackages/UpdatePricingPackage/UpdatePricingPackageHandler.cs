using Accounting.Application.Abstractions.Persistence;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.PricngPackages.UpdatePricingPackage
{
    public sealed class UpdatePricingPackageHandler : IRequestHandler<UpdatePricingPackageCommand>
    {
        private readonly IPricingPackageRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdatePricingPackageHandler(IPricingPackageRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(UpdatePricingPackageCommand request, CancellationToken cancellationToken)
        {
            var pricingPackage = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (pricingPackage is null)
            {
                throw new InvalidOperationException($"Pricing package with ID {request.Id} not found.");
            }

            pricingPackage.Update(
                name: request.Name,
                badge: request.Badge,
                description: request.Description,
                price: request.Price,
                priceLabel: request.PriceLabel,
                periodLabel: request.PeriodLabel,
                isRecommended: request.IsRecommended,
                isActive: request.IsActive,
                sortOrder: request.SortOrder
            );

            pricingPackage.ReplaceFeatures(request.Features);


            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
