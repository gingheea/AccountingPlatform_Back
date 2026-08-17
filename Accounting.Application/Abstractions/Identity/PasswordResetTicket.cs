namespace Accounting.Application.Abstractions.Identity
{
    /// <param name="Token">
    /// Одноразовий код від Identity. Він містить символи, які ламаються
    /// в адресному рядку, тому в посилання йде вже закодованим.
    /// </param>
    public sealed record PasswordResetTicket(string Email, string FullName, string Token);
}
