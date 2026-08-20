using Accounting.Application.Features.Testimonials.Common;
using Accounting.Domain.Enums;
using MediatR;
using System.Collections.Generic;

namespace Accounting.Application.Features.Testimonials.ListForAdmin
{
    public sealed record ListTestimonialsQuery(TestimonialStatus? Status)
        : IRequest<IReadOnlyList<TestimonialDto>>;
}
