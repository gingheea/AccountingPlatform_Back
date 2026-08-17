namespace Accounting.Domain.Enums
{
    /// <summary>
    /// Нумерація свідомо з 1, а не з 0. У C# незаданий enum дорівнює 0,
    /// тож якщо 0 означав би реальний стан, забуте присвоєння виглядало б
    /// як валідне значення. З одиниці «нуль у базі» одразу видно як помилку.
    /// </summary>
    public enum SubscriptionStatus
    {
        Active = 1,
        Paused = 2,
        Ended = 3
    }
}
