using Accounting.Application.Abstractions.Persistence;
using Accounting.Application.Features.PricingPackages.Common;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.PricngPackages.GetPricingPackageById
{
    public sealed class GetPricingPackageHandler : IRequestHandler<GetPricingPackageQuery, PricingPackageDto>
    {
        private readonly IPricingPackageRepository _pricingPackageRepository;
        private readonly IMapper _mapper;

        public GetPricingPackageHandler(IPricingPackageRepository pricingPackageRepository, IMapper mapper)
        {
            _pricingPackageRepository = pricingPackageRepository;
            _mapper = mapper;
        }

        public async Task<PricingPackageDto> Handle(GetPricingPackageQuery request, CancellationToken cancellationToken)
        {
            var pricingPackage = await _pricingPackageRepository.GetByIdAsync(request.Id, cancellationToken);
            return _mapper.Map<PricingPackageDto>(pricingPackage);
        }
    }
}
