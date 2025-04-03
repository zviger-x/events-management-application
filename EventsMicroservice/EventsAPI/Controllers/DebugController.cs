using Application.MediatR.Queries.EventCommentQueries;
using Application.UnitOfWork.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EventsAPI.Controllers
{
    [ApiController]
    [Route("api/debug")]
    public class DebugController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;

        public DebugController(IMediator mediator, IUnitOfWork unitOfWork)
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("event-comments")]
        public async Task<IActionResult> GetPagedCommentsAsync(CancellationToken cancellationToken = default)
        {
            var query = new EventCommentGetPagedQuery { PageNumber = 1, PageSize = int.MaxValue };
            var page = await _mediator.Send(query, cancellationToken);

            return Ok(page);
        }

        [HttpGet("seats")]
        public async Task<IActionResult> GetAllSeatsAsync(CancellationToken cancellationToken = default)
        {
            var seats = await _unitOfWork.SeatRepository.GetAllAsync(cancellationToken);

            return Ok(seats);
        }
    }
}
