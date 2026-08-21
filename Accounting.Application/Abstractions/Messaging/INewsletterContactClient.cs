using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Application.Abstractions.Messaging
{
    /// <summary>
    /// Adds a contact to the mail service's list so the accountant can send
    /// campaigns from its own interface without waiting for us to build one.
    /// Application does not know that this is Brevo specifically.
    /// </summary>
    public interface INewsletterContactClient
    {
        Task AddContactAsync(string email, CancellationToken ct);
    }
}
