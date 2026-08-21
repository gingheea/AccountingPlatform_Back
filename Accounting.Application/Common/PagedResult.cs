using System.Collections.Generic;

namespace Accounting.Application.Common
{
    /// <summary>
    /// One page of data together with the overall count.
    ///
    /// Total is what makes a page useful: without it the frontend cannot tell
    /// whether anything is left and would have to guess ("fewer came back than
    /// asked for, so that's the end"). That guess breaks as soon as a row is
    /// deleted between two requests.
    /// </summary>
    public sealed record PagedResult<T>(IReadOnlyList<T> Items, int Total);
}
