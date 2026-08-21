namespace Accounting.Domain.Enums
{
    /// <summary>
    /// Numbering deliberately starts at 1, not 0. An unset enum in C# equals 0,
    /// so if 0 meant a real state, a forgotten assignment would look like
    /// a valid value. Starting at 1 makes a zero in the database obviously wrong.
    /// </summary>
    public enum SubscriptionStatus
    {
        Active = 1,
        Paused = 2,
        Ended = 3
    }
}
