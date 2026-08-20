using Accounting.Application.Features.News.GetLatestNews;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.Api.Controllers
{
    [ApiController]
    [Route("api/news")]
    public class NewsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NewsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Новини бухгалтерської тематики з зовнішньої стрічки.
        ///
        /// Фронт міг би читати стрічку й сам, але не може: чужий сайт не віддає
        /// заголовок CORS, тому браузер такий запит блокує. Плюс тут кеш —
        /// один запит до джерела на всіх відвідувачів, а не на кожного.
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IReadOnlyList<NewsArticleDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<NewsArticleDto>>> Latest(
            [FromQuery] int take = 9,
            CancellationToken ct = default)
        {
            var news = await _mediator.Send(new GetLatestNewsQuery(take), ct);

            return Ok(news);
        }
    }
}
