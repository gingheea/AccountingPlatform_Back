using Accounting.Application.Abstractions.Persistence;
using Accounting.Application.Common;
using Accounting.Application.Features.ClientDocuments.Common;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Application.Features.ClientDocuments.ListDocuments
{
    public sealed class ListDocumentsHandler
        : IRequestHandler<ListDocumentsQuery, PagedResult<ClientDocumentDto>>
    {
        private readonly IClientDocumentRepository _repository;
        private readonly IMapper _mapper;

        public ListDocumentsHandler(IClientDocumentRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedResult<ClientDocumentDto>> Handle(
            ListDocumentsQuery request,
            CancellationToken ct)
        {
            var query = _repository.Query().AsNoTracking();

            if (request.UserId is not null)
                query = query.Where(x => x.UserId == request.UserId.Value);

            if (request.Category is not null)
                query = query.Where(x => x.Category == request.Category.Value);

            if (request.Direction is not null)
                query = query.Where(x => x.Direction == request.Direction.Value);

            if (request.Status is not null)
                query = query.Where(x => x.Status == request.Status.Value);

            return await query
                .OrderByDescending(x => x.CreatedAtUtc)
                .ThenBy(x => x.Id)
                .ToPagedResultAsync(
                    q => q.ProjectTo<ClientDocumentDto>(_mapper.ConfigurationProvider),
                    request.Page,
                    request.PageSize,
                    ct);
        }
    }
}
