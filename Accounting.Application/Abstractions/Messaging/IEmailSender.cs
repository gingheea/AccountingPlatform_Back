using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Application.Abstractions.Messaging
{
    /// <summary>
    /// Контракт навмисне нічого не знає про SMTP: заміна провайдера або перехід
    /// на HTTP API не має зачіпати жоден обробник в Application.
    /// </summary>
    public interface IEmailSender
    {
        Task SendAsync(EmailMessage message, CancellationToken ct);
    }
}
