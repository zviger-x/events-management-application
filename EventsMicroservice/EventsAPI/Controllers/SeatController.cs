using Application.MediatR.Queries.SeatQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EventsAPI.Controllers
{
    [ApiController]
    [Route("api/seats")]
    public class SeatController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SeatController(IMediator mediator)
        {
            _mediator = mediator;
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
