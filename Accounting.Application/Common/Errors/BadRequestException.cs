using System;

namespace Accounting.Application.Common.Errors
{
    /// <summary>
    /// Користувач передав щось некоректне, і йому треба це показати: невірний
    /// поточний пароль, протерміноване посилання відновлення тощо.
    /// Без такого типу подібні випадки падали в загальний обробник і клієнт
    /// бачив «Unexpected error» замість причини.
    /// </summary>
    public sealed class BadRequestException : Exception
    {
        public BadRequestException(string message) : base(message) { }
    }
}
