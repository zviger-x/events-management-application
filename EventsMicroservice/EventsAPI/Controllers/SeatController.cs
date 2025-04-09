using Application.MediatR.Queries.SeatQueries;
using Application.UnitOfWork.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EventsAPI.Controllers
{
    [ApiController]
    [Route("api/seats")]
    public class SeatController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;

        public SeatController(IMediator mediator, IUnitOfWork unitOfWork)
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("/api/events/{eventId}/seats")]
        public async Task<IActionResult> GetEventSeats([FromRoute] Guid eventId, CancellationToken token)
        {
            var query = new SeatGetByEventQuery { EventId = eventId };

            var seats = await _mediator.Send(query, token);

            return Ok(seats);
        }
    }
}
