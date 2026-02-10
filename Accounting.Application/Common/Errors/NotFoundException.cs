namespace Accounting.Application.Common.Errors;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}
