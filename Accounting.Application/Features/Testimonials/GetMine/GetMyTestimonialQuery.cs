using Accounting.Application.Features.Testimonials.Common;
using MediatR;

namespace Accounting.Application.Features.Testimonials.GetMine
{
    /// <summary>Returns null when the client has not left a testimonial yet; not an error.</summary>
    public sealed record GetMyTestimonialQuery : IRequest<TestimonialDto?>;
}
