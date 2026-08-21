using System;

namespace Accounting.Application.Common.Errors
{
    /// <summary>
    /// The caller sent something invalid and needs to be told what: a wrong
    /// current password, an expired recovery link and so on.
    /// Without this type such cases fell into the generic handler and the client
    /// saw "Unexpected error" instead of the reason.
    /// </summary>
    public sealed class BadRequestException : Exception
    {
        public BadRequestException(string message) : base(message) { }
    }
}
