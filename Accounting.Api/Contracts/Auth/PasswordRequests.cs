namespace Accounting.Api.Contracts.Auth
{
    public sealed record ForgotPasswordRequest(string Email);

    public sealed record ResetPasswordRequest(string Email, string Token, string NewPassword);

    public sealed record ChangeOwnPasswordRequest(string CurrentPassword, string NewPassword);
}
