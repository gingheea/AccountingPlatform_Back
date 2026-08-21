using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Application.Common
{
    public static class Pagination
    {
        public const int DefaultPageSize = 20;

        /// <summary>
        /// Upper bound on page size. Page number and size arrive in the query string,
        /// meaning anyone outside controls them. Without a cap ?pageSize=1000000
        /// would be a way to take the database down with a single request.
        /// </summary>
        public const int MaxPageSize = 200;

        public static int NormalizePage(int page) => page < 1 ? 1 : page;

        public static int NormalizePageSize(int pageSize)
            => pageSize < 1 ? DefaultPageSize : Math.Min(pageSize, MaxPageSize);

        /// <summary>
        /// Counts the total and returns a single page.
        ///
        /// The caller supplies the ordering, and it must be unambiguous.
        /// Ordering by date alone lets the database return two rows sharing a date
        /// in a different order on each request: then one row lands on two pages
        /// while another never appears at all. That is why every call adds Id
        /// as the second ordering field.
        ///
        /// <paramref name="project"/> is a separate step because some handlers project
        /// through AutoMapper (ProjectTo) and others by hand (Select).
        /// </summary>
        public static async Task<PagedResult<TResult>> ToPagedResultAsync<TSource, TResult>(
            this IOrderedQueryable<TSource> ordered,
            Func<IQueryable<TSource>, IQueryable<TResult>> project,
            int page,
            int pageSize,
            CancellationToken ct)
        {
            page = NormalizePage(page);
            pageSize = NormalizePageSize(pageSize);

            // Counted before Skip/Take: Total must be the number of all rows,
            // not of the ones that landed on this page.
            var total = await ordered.CountAsync(ct);

            var pageQuery = ordered
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            var items = await project(pageQuery).ToListAsync(ct);

            return new PagedResult<TResult>(items, total);
        }
    }
}
