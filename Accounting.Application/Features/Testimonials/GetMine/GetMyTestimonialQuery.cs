using Accounting.Application.Features.Testimonials.Common;
using MediatR;

namespace Accounting.Application.Features.Testimonials.GetMine
{
    /// <summary>Повертає null, якщо клієнт ще не лишав відгуку — це не помилка.</summary>
    public sealed record GetMyTestimonialQuery : IRequest<TestimonialDto?>;
}
