using Accounting.Application.Abstractions.Persistence;
using Accounting.Application.Features.PricingPackages.Common;
using Accounting.Application.Features.Services.Common;
using Accounting.Application.Features.Services.List;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.PricngPackages.GetPricingPackages
{
    public sealed class GetPricingPackagesHandler : IRequestHandler<GetPricingPackagesQuery, IReadOnlyList<PricingPackageDto>>
    {
        private readonly IPricingPackageRepository _pricingPackageRepository;
        public readonly IMapper _mapper;

        public GetPricingPackagesHandler(IPricingPackageRepository pricingPackageRepository, IMapper mapper)
        {
            _pricingPackageRepository = pricingPackageRepository ?? throw new ArgumentNullException(nameof(pricingPackageRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IReadOnlyList<PricingPackageDto>> Handle(GetPricingPackagesQuery request, CancellationToken cancellationToken)
        {
            return await _pricingPackageRepository.Query()
            .AsNoTracking()
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Name)
            .ProjectTo<PricingPackageDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
        }
    }
}
