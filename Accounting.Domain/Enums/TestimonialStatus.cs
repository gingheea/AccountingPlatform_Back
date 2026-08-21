namespace Accounting.Domain.Enums;

/// <summary>
/// Numbering starts at 1 on purpose: if the value is ever left unset,
/// the field holds 0, which reads as an error rather than as "Pending".
/// </summary>
public enum TestimonialStatus
{
    Pending = 1,
    Approved = 2,
    Rejected = 3
}
