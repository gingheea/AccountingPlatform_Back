using MediatR;
using System;

namespace Accounting.Application.Features.Testimonials.Submit
{
    /// <summary>
    /// UserId у команді немає навмисно: його бере обробник із токена.
    /// Якби він приходив у тілі запиту, будь-хто міг би написати відгук
    /// від чужого імені.
    /// </summary>
    public sealed record SubmitTestimonialCommand(string Content, string? AuthorRole)
        : IRequest<Guid>;
}
