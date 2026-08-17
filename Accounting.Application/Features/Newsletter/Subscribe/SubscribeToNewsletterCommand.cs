using MediatR;

namespace Accounting.Application.Features.Newsletter.Subscribe
{
    public sealed record SubscribeToNewsletterCommand(string Email, string? Source) : IRequest;
}
