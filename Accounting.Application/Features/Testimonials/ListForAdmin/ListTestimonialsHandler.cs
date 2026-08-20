using Accounting.Application.Abstractions.Persistence;
using Accounting.Application.Features.Testimonials.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Testimonials.ListForAdmin
{
    public sealed class ListTestimonialsHandler
        : IRequestHandler<ListTestimonialsQuery, IReadOnlyList<TestimonialDto>>
    {
        private readonly ITestimonialRepository _repository;

        public ListTestimonialsHandler(ITestimonialRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<TestimonialDto>> Handle(
            ListTestimonialsQuery request,
            CancellationToken ct)
        {
            var query = _repository.Query().AsNoTracking();

            if (request.Status is not null)
                query = query.Where(x => x.Status == request.Status.Value);

            // Найновіші зверху: бухгалтер відкриває список, щоб розглянути свіже.
            return await query
                .OrderByDescending(x => x.CreatedAtUtc)
                .Select(x => new TestimonialDto(
                    x.Id,
                    x.UserId,
                    x.AuthorName,
                    x.AuthorRole,
                    x.Content,
                    x.Status,
                    x.ModerationNote,
                    x.CreatedAtUtc,
                    x.ModeratedAtUtc))
                .ToListAsync(ct);
        }
    }
}
