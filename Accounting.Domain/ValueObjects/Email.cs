using System.Net.Mail;
using Accounting.Domain.Exceptions;

namespace Accounting.Domain.ValueObjects;

public sealed class Email : IEquatable<Email>
{
    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Email Create(string input)
    {
        input = input?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(input))
            throw new DomainException("Email is required.");

        if (input.Length > 200)
            throw new DomainException("Email max length is 200.");

        try
        {
            var mailAddress = new MailAddress(input);

            if (!string.Equals(mailAddress.Address, input, StringComparison.OrdinalIgnoreCase))
                throw new DomainException("Email format is invalid.");
        }
        catch
        {
            throw new DomainException("Email format is invalid.");
        }

        return new Email(input);
    }

    public override string ToString() => Value;

    public bool Equals(Email? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj) => obj is Email other && Equals(other);

    public override int GetHashCode() => Value.ToUpperInvariant().GetHashCode();

    public static implicit operator string(Email email) => email.Value;
}