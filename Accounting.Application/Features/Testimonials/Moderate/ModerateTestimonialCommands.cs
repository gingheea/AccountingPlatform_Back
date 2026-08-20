using MediatR;
using System;

namespace Accounting.Application.Features.Testimonials.Moderate
{
    public sealed record ApproveTestimonialCommand(Guid Id) : IRequest;

    public sealed record RejectTestimonialCommand(Guid Id, string? Note) : IRequest;

    public sealed record DeleteTestimonialCommand(Guid Id) : IRequest;
}
