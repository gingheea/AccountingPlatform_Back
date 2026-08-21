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
        /// Стеля розміру сторінки. Номер сторінки й розмір приходять із рядка
        /// запиту, тобто ними керує будь-хто ззовні. Без стелі ?pageSize=1000000
        /// був би способом покласти базу одним запитом.
        /// </summary>
        public const int MaxPageSize = 200;

        public static int NormalizePage(int page) => page < 1 ? 1 : page;

        public static int NormalizePageSize(int pageSize)
            => pageSize < 1 ? DefaultPageSize : Math.Min(pageSize, MaxPageSize);

        /// <summary>
        /// Рахує загальну кількість і повертає одну сторінку.
        ///
        /// Сортування задає той, хто викликає — і воно має бути однозначним.
        /// Якщо сортувати лише за датою, два записи з однаковою датою база
        /// вільна повертати в різному порядку на різних запитах: тоді один
        /// запис приїде на двох сторінках, а інший не приїде взагалі. Тому
        /// в кожному виклику другим полем іде Id.
        ///
        /// <paramref name="project"/> окремим кроком, бо частина обробників
        /// проєктує через AutoMapper (ProjectTo), а частина — руками (Select).
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

            // Рахуємо до Skip/Take: Total має бути кількістю всіх записів,
            // а не тих, що потрапили на цю сторінку.
            var total = await ordered.CountAsync(ct);

            var pageQuery = ordered
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            var items = await project(pageQuery).ToListAsync(ct);

            return new PagedResult<TResult>(items, total);
        }
    }
}
