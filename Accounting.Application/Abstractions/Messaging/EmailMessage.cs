namespace Accounting.Application.Abstractions.Messaging
{
    /// <summary>
    /// Один лист. Окремий тип, а не купа параметрів у методі: полів з часом
    /// стає більше (вкладення, копія), і кожне нове не ламатиме виклики.
    /// </summary>
    /// <param name="TextBody">
    /// Текстова версія. Лист лише з HTML частіше вважають спамом.
    /// </param>
    /// <param name="ReplyTo">
    /// Куди піде відповідь, якщо натиснути «Відповісти». Для сповіщення про
    /// заявку сюди кладемо пошту клієнта — тоді бухгалтер відповідає йому напряму.
    /// </param>
    public sealed record EmailMessage(
        string To,
        string Subject,
        string HtmlBody,
        string? TextBody = null,
        string? ReplyTo = null,
        string? ReplyToName = null
    );
}
