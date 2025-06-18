using Application.Contracts;
using Application.MediatR.Commands.EventCommands;
using Application.MediatR.Queries.EventQueries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Attributes;
using Shared.Common;
using Shared.Enums;

namespace EventsAPI.Controllers
{
    [ApiController]
    [Route("api/events")]
    [AuthorizeRoles(UserRoles.EventManager, UserRoles.Admin)]
    public class EventController : ControllerBase
    {
        private const int PageSize = 10;

        private readonly IMediator _mediator;

        public EventController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateEventDto eventToCreate, CancellationToken cancellationToken)
        {
            var command = new EventCreateCommand { Event = eventToCreate };
            var createdEventId = await _mediator.Send(command, cancellationToken);

            return StatusCode(StatusCodes.Status201Created, new { Id = createdEventId });
        }

        [HttpPut("{eventId}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid eventId, [FromBody] UpdateEventDto eventToUpdate, CancellationToken cancellationToken)
        {
            var command = new EventUpdateCommand { EventId = eventId, Event = eventToUpdate };
            await _mediator.Send(command, cancellationToken);

            return Ok();
        }

        [HttpDelete("{eventId}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid eventId, CancellationToken cancellationToken)
        {
            var command = new EventDeleteCommand { Id = eventId };
            await _mediator.Send(command, cancellationToken);

            return NoContent();
        }

        [AllowAnonymous]
        [HttpGet("{eventId}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] Guid eventId, CancellationToken cancellationToken)
        {
            var query = new EventGetByIdQuery { Id = eventId };
            var @event = await _mediator.Send(query, cancellationToken);

            return Ok(@event);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetPagedAsync(
            [FromQuery] string name,
            [FromQuery] string description,
            [FromQuery] string location,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] int pageNumber = 1,
            CancellationToken cancellationToken = default)
        {
            var pageParameters = new PageParameters
            {
                PageNumber = pageNumber,
                PageSize = PageSize
            };

            var query = new EventGetPagedQuery
            {
                Name = name?.Trim(),
                Description = description?.Trim(),
                Location = location?.Trim(),
                FromDate = fromDate,
                ToDate = toDate,
                PageParameters = pageParameters
            };

            var page = await _mediator.Send(query, cancellationToken);

            return Ok(page);
        }
    }
}
