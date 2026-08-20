using Accounting.Application.Features.Testimonials.Common;
using MediatR;
using System.Collections.Generic;

namespace Accounting.Application.Features.Testimonials.ListPublished
{
    public sealed record ListPublishedTestimonialsQuery(int Take)
        : IRequest<IReadOnlyList<PublicTestimonialDto>>;
}
