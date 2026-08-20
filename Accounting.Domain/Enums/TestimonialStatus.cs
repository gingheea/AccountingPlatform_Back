namespace Accounting.Domain.Enums;

/// <summary>
/// Нумерація починається з 1 навмисно: якщо значення десь забудуть проставити,
/// у полі опиниться 0, і це одразу видно як помилку, а не як «На розгляді».
/// </summary>
public enum TestimonialStatus
{
    Pending = 1,
    Approved = 2,
    Rejected = 3
}
