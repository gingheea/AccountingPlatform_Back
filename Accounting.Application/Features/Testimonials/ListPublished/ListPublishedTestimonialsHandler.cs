using Accounting.Application.Abstractions.Persistence;
using Accounting.Application.Common;
using Accounting.Application.Features.Testimonials.Common;
using Accounting.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Testimonials.ListPublished
{
    public sealed class ListPublishedTestimonialsHandler
        : IRequestHandler<ListPublishedTestimonialsQuery, PagedResult<PublicTestimonialDto>>
    {
        private const int MaxTake = 24;

        private readonly ITestimonialRepository _repository;

        public ListPublishedTestimonialsHandler(ITestimonialRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResult<PublicTestimonialDto>> Handle(
            ListPublishedTestimonialsQuery request,
            CancellationToken ct)
        {
            var take = Math.Clamp(request.Take, 1, MaxTake);
            var skip = Math.Max(request.Skip, 0);

            // The Approved filter lives here, the single place the public page reads
            // from. If the frontend did the filtering, anyone could see pending
            // testimonials just by opening the raw response.
            var query = _repository.Query()
                .AsNoTracking()
                .Where(x => x.Status == TestimonialStatus.Approved);

            // Counted before Skip/Take: Total must be the number of all approved
            // testimonials, not of the ones that landed on this page.
            var total = await query.CountAsync(ct);

            var items = await query
                // The ordering must be unambiguous, otherwise a row can land on two pages
                // or on none at all.
                // Two testimonials can share ModeratedAtUtc down to the millisecond,
                // so Id is added as the final tie-breaker.
                .OrderByDescending(x => x.ModeratedAtUtc)
                .ThenBy(x => x.Id)
                .Skip(skip)
                .Take(take)
                .Select(x => new PublicTestimonialDto(
                    x.Id, x.AuthorName, x.AuthorRole, x.Content, x.CreatedAtUtc))
                .ToListAsync(ct);

            return new PagedResult<PublicTestimonialDto>(items, total);
        }
    }
}
