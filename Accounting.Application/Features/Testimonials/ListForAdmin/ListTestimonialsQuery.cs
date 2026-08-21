using Accounting.Application.Common;
using Accounting.Application.Features.Testimonials.Common;
using Accounting.Domain.Enums;
using MediatR;

namespace Accounting.Application.Features.Testimonials.ListForAdmin
{
    public sealed record ListTestimonialsQuery(TestimonialStatus? Status, int Page, int PageSize)
        : IRequest<PagedResult<TestimonialDto>>;
}
