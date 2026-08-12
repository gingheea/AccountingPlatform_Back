namespace Accounting.Application.Common.Options
{
    /// <summary>
    /// «Кому і куди» — це рішення бізнесу, а не транспорту, тому воно живе тут,
    /// а не в SmtpOptions. Зміна поштового провайдера не має чіпати ці значення.
    /// </summary>
    public sealed class NotificationOptions
    {
        public const string SectionName = "Notifications";

        /// <summary>Пошта бухгалтера — отримувач сповіщень про нові заявки.</summary>
        public string AccountantEmail { get; init; } = string.Empty;

        /// <summary>Посилання на список заявок в адмінці, щоб з листа можна було одразу перейти.</summary>
        public string AdminRequestsUrl { get; init; } = string.Empty;
    }
}
