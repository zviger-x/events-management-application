using Application.Contracts;
using Application.MediatR.Commands.EventCommands;
using Application.MediatR.Queries.EventQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Attributes;
using Shared.Enums;
using Shared.Common;

namespace EventsAPI.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventController : Controller
    {
        private const int PageSize = 10;

        private readonly IMediator _mediator;

        public EventController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [AuthorizeRoles(UserRoles.EventManager, UserRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateEventDTO eventToCreate, CancellationToken cancellationToken)
        {
            var command = new EventCreateCommand { Event = eventToCreate };
            var createdEventId = await _mediator.Send(command, cancellationToken);

            return Ok(createdEventId);
        }

        [AuthorizeRoles(UserRoles.EventManager, UserRoles.Admin)]
        [HttpPut("{eventId}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid eventId, [FromBody] UpdateEventDTO eventToUpdate, CancellationToken cancellationToken)
        {
            if (eventId != eventToUpdate.Id)
                throw new ArgumentException("You are not allowed to modify this event.");

            var command = new EventUpdateCommand { Event = eventToUpdate };
            await _mediator.Send(command, cancellationToken);

            return Ok();
        }

        [AuthorizeRoles(UserRoles.EventManager, UserRoles.Admin)]
        [HttpDelete("{eventId}")]
        public async Task<IActionResult> DeleteAsync(Guid eventId, CancellationToken cancellationToken)
        {
            var command = new EventDeleteCommand { Id = eventId };
            await _mediator.Send(command, cancellationToken);

            return Ok();
        }

        [HttpGet("{eventId}")]
        public async Task<IActionResult> GetByIdAsync(Guid eventId, CancellationToken cancellationToken)
        {
            var query = new EventGetByIdQuery { Id = eventId };
            var @event = await _mediator.Send(query, cancellationToken);

            if (@event == null)
                return NotFound();

            return Ok(@event);
        }

        [HttpGet]
        public async Task<IActionResult> GetPagedAsync([FromQuery] int pageNumber = 1, CancellationToken cancellationToken = default)
        {
            var pageParameters = new PageParameters { PageNumber = pageNumber, PageSize = PageSize };
            var query = new EventGetPagedQuery { PageParameters = pageParameters };

            var page = await _mediator.Send(query, cancellationToken);

            return Ok(page);
        }
    }
}
