using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Application.Abstractions.Messaging
{
    /// <summary>
    /// The contract deliberately knows nothing about SMTP: swapping provider or
    /// moving to an HTTP API must not touch a single Application handler.
    /// </summary>
    public interface IEmailSender
    {
        Task SendAsync(EmailMessage message, CancellationToken ct);
    }
}
