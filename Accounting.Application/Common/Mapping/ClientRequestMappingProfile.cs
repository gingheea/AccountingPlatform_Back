using Accounting.Application.Features.ClientRequests.Common;
using Accounting.Domain.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Common.Mapping
{
    public sealed class ClientRequestMappingProfile : Profile
    {
        public ClientRequestMappingProfile()
        {
            CreateMap<ClientRequest, ClientRequestDto>();
        }
    }
}
