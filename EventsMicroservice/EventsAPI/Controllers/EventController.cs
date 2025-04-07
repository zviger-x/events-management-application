using Application.Contracts;
using Application.MediatR.Commands.EventCommands;
using Application.MediatR.Commands.EventCommentCommands;
using Application.MediatR.Queries.EventQueries;
using Application.MediatR.Queries.EventCommentQueries;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EventsAPI.Attributes;

namespace EventsAPI.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventController : Controller
    {
        private readonly IMediator _mediator;

        private const int EventsPageSize = 10;
        private const int CommentsPageSize = 10;

        public EventController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// ---- Events ----

        [AuthorizeRoles(UserRoles.EventManager, UserRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateEventDTO eventToCreate, CancellationToken cancellationToken)
        {
            var command = new EventCreateCommand { Event = eventToCreate };
            var createdEventId = await _mediator.Send(command, cancellationToken);

            return Ok(createdEventId);
        }

        [AuthorizeRoles(UserRoles.EventManager, UserRoles.Admin)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] UpdateEventDTO eventToUpdate, CancellationToken cancellationToken)
        {
            if (id != eventToUpdate.Id)
                throw new ArgumentException("You are not allowed to modify this event.");

            var command = new EventUpdateCommand { Event = eventToUpdate };
            await _mediator.Send(command, cancellationToken);

            return Ok();
        }

        [AuthorizeRoles(UserRoles.EventManager, UserRoles.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var command = new EventDeleteCommand { Id = id };
            await _mediator.Send(command, cancellationToken);

            return Ok();
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var query = new EventGetByIdQuery { Id = id };
            var @event = await _mediator.Send(query, cancellationToken);

            if (@event == null)
                return NotFound();

            return Ok(@event);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetPagedAsync([FromQuery] int pageNumber = 1, CancellationToken cancellationToken = default)
        {
            var query = new EventGetPagedQuery { PageNumber = pageNumber, PageSize = EventsPageSize };
            var page = await _mediator.Send(query, cancellationToken);

            return Ok(page);
        }

        /// ---- Event comments ----

        [Authorize]
        [HttpPost("{eventId}/comments")]
        public async Task<IActionResult> CreateCommentAsync([FromRoute] Guid eventId, [FromBody] EventComment commentToCreate, CancellationToken token)
        {
            if (eventId != commentToCreate.EventId)
                throw new ArgumentException("You are not allowed to create a comment for this event.");

            var command = new EventCommentCreateCommand { EventComment = commentToCreate };
            var createdCommentId = await _mediator.Send(command, token);

            return Ok(createdCommentId);
        }

        [Authorize]
        [HttpPut("{eventId}/comments/{commentId}")]
        public async Task<IActionResult> UpdateCommentAsync([FromRoute] Guid eventId, [FromRoute] Guid commentId, [FromBody] EventComment commentToUpdate, CancellationToken token)
        {
            // TODO: Добавить проверку, что пользователь является создателем отзыва.

            if (eventId != commentToUpdate.EventId)
                throw new ArgumentException("You are not allowed to edit the comment for this event.");

            if (commentId != commentToUpdate.Id)
                throw new ArgumentException("You are not allowed to modify this comment.");

            var command = new EventCommentUpdateCommand { EventComment = commentToUpdate };
            await _mediator.Send(command, token);

            return Ok();
        }

        [Authorize]
        [HttpDelete("{eventId}/comments/{commentId}")]
        public async Task<IActionResult> DeleteCommentAsync([FromRoute] Guid eventId, [FromRoute] Guid commentId, CancellationToken token)
        {
            // TODO: Добавить проверку, что пользователь является создателем отзыва.

            // if (id != commentToUpdate.EventId)
            //     throw new ArgumentException("You are not allowed to edit the comment for this event.");

            var command = new EventCommentDeleteCommand { Id = commentId };
            await _mediator.Send(command, token);

            return Ok();
        }

        [Authorize]
        [HttpGet("{eventId}/comments")]
        public async Task<IActionResult> GetPagedCommentsAsync([FromRoute] Guid eventId, [FromQuery] int pageNumber = 1, CancellationToken cancellationToken = default)
        {
            var command = new EventCommentGetPagedByEventQuery { EventId = eventId, PageNumber = pageNumber, PageSize = CommentsPageSize };
            var page = await _mediator.Send(command, cancellationToken);

            return Ok(page);
        }
    }
}
