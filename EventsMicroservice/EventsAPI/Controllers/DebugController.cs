using Application.MediatR.Queries.ReviewQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EventsAPI.Controllers
{
    [ApiController]
    [Route("api/debug")]
    public class DebugController : Controller
    {
        private readonly IMediator _mediator;

        private const int PageSize = int.MaxValue;

        public DebugController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("reviews")]
        public async Task<IActionResult> GetPagedReviewsAsync([FromQuery] int pageNumber = 1, CancellationToken cancellationToken = default)
        {
            var query = new ReviewGetPagedQuery { PageNumber = pageNumber, PageSize = PageSize };
            var page = await _mediator.Send(query);

            return Ok(page);
        }
    }
}
