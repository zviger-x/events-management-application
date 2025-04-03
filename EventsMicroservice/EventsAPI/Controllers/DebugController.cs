using Application.MediatR.Queries.EventCommentQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EventsAPI.Controllers
{
    [ApiController]
    [Route("api/debug")]
    public class DebugController : Controller
    {
        private readonly IMediator _mediator;

        public DebugController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("event-comments")]
        public async Task<IActionResult> GetPagedCommentsAsync(CancellationToken cancellationToken = default)
        {
            var query = new EventCommentGetPagedQuery { PageNumber = 1, PageSize = int.MaxValue };
            var page = await _mediator.Send(query, cancellationToken);

            return Ok(page);
        }
    }
}
