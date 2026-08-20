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

            // Фільтр за Approved стоїть тут, у єдиному місці, звідки публічна
            // сторінка бере дані. Якби відбір робив фронт, будь-хто побачив би
            // нерозглянуті відгуки, просто відкривши відповідь запиту.
            var query = _repository.Query()
                .AsNoTracking()
                .Where(x => x.Status == TestimonialStatus.Approved);

            // Рахуємо до Skip/Take: Total має бути кількістю всіх схвалених
            // відгуків, а не тих, що потрапили на цю сторінку.
            var total = await query.CountAsync(ct);

            var items = await query
                // Сортування має бути однозначним, інакше запис може
                // потрапити на дві сторінки або не потрапити на жодну.
                // ModeratedAtUtc у двох відгуків може збігтися до мілісекунди,
                // тому додаємо Id як остаточний розділювач.
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
