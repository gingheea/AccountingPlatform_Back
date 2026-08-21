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
        /// Accounting news pulled from an external feed.
        ///
        /// The frontend could read the feed itself but cannot: the other site sends
        /// no CORS header, so the browser blocks the request. There is also a cache:
        /// one call to the source for all visitors instead of one per visitor.
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
