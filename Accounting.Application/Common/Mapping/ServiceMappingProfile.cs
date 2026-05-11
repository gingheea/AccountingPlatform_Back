using Accounting.Application.Features.ClientRequests.Common;
using Accounting.Application.Features.Services.Common;
using Accounting.Domain.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Common.Mapping
{
    public sealed class ServiceMappingProfile : Profile
    {
        public ServiceMappingProfile()
        {
            CreateMap<Service, ServiceDto>()
             .ForCtorParam(
                 "Tags",
                 opt => opt.MapFrom(src => src.Tags
                     .OrderBy(tag => tag.SortOrder)
                     .Select(tag => tag.Name)
                 )
             );
        }
    }
}
