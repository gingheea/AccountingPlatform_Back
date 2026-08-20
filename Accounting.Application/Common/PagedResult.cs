using System.Collections.Generic;

namespace Accounting.Application.Common
{
    /// <summary>
    /// Сторінка даних разом із загальною кількістю.
    ///
    /// Саме Total робить сторінку корисною: без нього фронт не знає, чи є
    /// що вантажити далі, і мусив би здогадуватись — «прийшло менше, ніж
    /// просили, отже кінець». Це працює, доки хтось не видалить запис
    /// між двома запитами.
    /// </summary>
    public sealed record PagedResult<T>(IReadOnlyList<T> Items, int Total);
}
