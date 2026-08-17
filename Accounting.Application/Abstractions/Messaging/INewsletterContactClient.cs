using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Application.Abstractions.Messaging
{
    /// <summary>
    /// Додає контакт у список розсилки поштового сервісу, щоб бухгалтер міг
    /// слати листи з його інтерфейсу, не чекаючи, поки ми напишемо свій.
    /// Application не знає, що це саме Brevo.
    /// </summary>
    public interface INewsletterContactClient
    {
        Task AddContactAsync(string email, CancellationToken ct);
    }
}
