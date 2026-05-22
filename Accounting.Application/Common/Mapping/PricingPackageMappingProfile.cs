using Accounting.Application.Features.PricingPackages.Common;
using Accounting.Domain.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Common.Mapping
{
    public sealed class PricingPackageMappingProfile : Profile
    {
        public PricingPackageMappingProfile()
        {
            CreateMap<PricingPackage, PricingPackageDto>();
        }
    }
}
