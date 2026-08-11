using Accounting.Application.Features.ClientDocuments.Common;
using Accounting.Domain.Entities;
using AutoMapper;

namespace Accounting.Application.Common.Mapping
{
    public sealed class ClientDocumentMappingProfile : Profile
    {
        public ClientDocumentMappingProfile()
        {
            // StorageKey is intentionally not exposed — clients get a pre-signed URL instead.
            CreateMap<ClientDocument, ClientDocumentDto>();
        }
    }
}
