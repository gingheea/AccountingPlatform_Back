using Accounting.Application.Common;
using Accounting.Application.Features.Testimonials.Common;
using MediatR;

namespace Accounting.Application.Features.Testimonials.ListPublished
{
    public sealed record ListPublishedTestimonialsQuery(int Skip, int Take)
        : IRequest<PagedResult<PublicTestimonialDto>>;
}
