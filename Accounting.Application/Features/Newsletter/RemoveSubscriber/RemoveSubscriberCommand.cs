using MediatR;
using System;

namespace Accounting.Application.Features.Newsletter.RemoveSubscriber
{
    /// <summary>
    /// Повне видалення, а не позначка «відписаний»: якщо людина просить
    /// прибрати її пошту, лишати запис у базі не можна.
    /// </summary>
    public sealed record RemoveSubscriberCommand(Guid Id) : IRequest;
}
