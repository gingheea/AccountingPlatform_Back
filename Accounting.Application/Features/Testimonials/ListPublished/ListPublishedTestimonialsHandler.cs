using Accounting.Application.Abstractions.Persistence;
using Accounting.Application.Features.Testimonials.Common;
using Accounting.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Testimonials.ListPublished
{
    public sealed class ListPublishedTestimonialsHandler
        : IRequestHandler<ListPublishedTestimonialsQuery, IReadOnlyList<PublicTestimonialDto>>
    {
        private const int MaxTake = 24;

        private readonly ITestimonialRepository _repository;

        public ListPublishedTestimonialsHandler(ITestimonialRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<PublicTestimonialDto>> Handle(
            ListPublishedTestimonialsQuery request,
            CancellationToken ct)
        {
            var take = Math.Clamp(request.Take, 1, MaxTake);

            // Фільтр за Approved стоїть тут, у єдиному місці, звідки публічна
            // сторінка бере дані. Якби відбір робив фронт, будь-хто побачив би
            // нерозглянуті відгуки, просто відкривши відповідь запиту.
            return await _repository.Query()
                .AsNoTracking()
                .Where(x => x.Status == TestimonialStatus.Approved)
                .OrderByDescending(x => x.ModeratedAtUtc)
                .Take(take)
                .Select(x => new PublicTestimonialDto(
                    x.Id, x.AuthorName, x.AuthorRole, x.Content, x.CreatedAtUtc))
                .ToListAsync(ct);
        }
    }
}
