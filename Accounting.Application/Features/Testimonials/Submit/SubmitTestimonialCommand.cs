using MediatR;
using System;

namespace Accounting.Application.Features.Testimonials.Submit
{
    /// <summary>
    /// The command has no UserId on purpose: the handler takes it from the token.
    /// If it arrived in the request body, anyone could post a testimonial
    /// under somebody else's name.
    /// </summary>
    public sealed record SubmitTestimonialCommand(string Content, string? AuthorRole)
        : IRequest<Guid>;
}
