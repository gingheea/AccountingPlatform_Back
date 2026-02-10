namespace Accounting.Application.Common.Errors;

public class ForbiddenException : Exception
{
    public ForbiddenException(string message) : base(message) { }
}
