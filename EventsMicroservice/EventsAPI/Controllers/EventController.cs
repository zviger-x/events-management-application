using Application.Contracts;
using Application.MediatR.Commands.EventCommands;
using Application.MediatR.Queries.EventQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EventsAPI.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventController : Controller
    {
        private readonly IMediator _mediator;

        private const int PageSize = 10;

        public EventController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] EventDTO eventToCreate, CancellationToken cancellationToken)
        {
            var command = new EventCreateCommand { Event = eventToCreate };
            await _mediator.Send(command);

            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] EventDTO eventToUpdate, CancellationToken cancellationToken)
        {
            if (id != eventToUpdate.Id)
                throw new ArgumentException("You are not allowed to modify this event.");

            var command = new EventUpdateCommand { Event = eventToUpdate };
            await _mediator.Send(command);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var command = new EventDeleteCommand { Id = id };
            await _mediator.Send(command);

            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var query = new EventGetByIdQuery { Id = id };
            var @event = await _mediator.Send(query);

            if (@event == null)
                return NotFound();

            return Ok(@event);
        }

        [HttpGet]
        public async Task<IActionResult> GetPagedAsync([FromQuery] int pageNumber = 1, CancellationToken cancellationToken = default)
        {
            var query = new EventGetPagedQuery { PageNumber = pageNumber, PageSize = PageSize };
            var page = await _mediator.Send(query);

            return Ok(page);
        }
    }
}
