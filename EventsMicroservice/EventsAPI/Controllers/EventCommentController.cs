using Application.Contracts;
using Application.MediatR.Commands.EventCommentCommands;
using Application.MediatR.Queries.EventCommentQueries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Common;
using Shared.Extensions;

namespace EventsAPI.Controllers
{
    [ApiController]
    [Route("api/event-comments")]
    public class EventCommentController : Controller
    {
        private const int PageSize = 10;

        private readonly IMediator _mediator;

        public EventCommentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [HttpPost("/api/events/{eventId}/comments")]
        public async Task<IActionResult> CreateCommentAsync([FromRoute] Guid eventId, [FromBody] CreateEventCommentDto commentToCreate, CancellationToken token)
        {
            var command = new EventCommentCreateCommand { EventId = eventId, EventComment = commentToCreate };
            var createdCommentId = await _mediator.Send(command, token);

            return StatusCode(StatusCodes.Status201Created, new { Id = createdCommentId });
        }

        [Authorize]
        [HttpPatch("/api/events/{eventId}/comments/{commentId}")]
        public async Task<IActionResult> UpdateCommentAsync([FromRoute] Guid eventId, [FromRoute] Guid commentId, [FromBody] UpdateEventCommentDto commentToUpdate, CancellationToken token)
        {
            var userId = User.GetUserIdOrThrow();
            var isAdmin = User.IsAdminOrThrow();

            var command = new EventCommentUpdateCommand
            {
                CurrentUserId = userId,
                IsCurrentUserAdmin = isAdmin,
                EventId = eventId,
                CommentId = commentId,
                EventComment = commentToUpdate
            };

            await _mediator.Send(command, token);

            return Ok();
        }

        [Authorize]
        [HttpDelete("/api/events/{eventId}/comments/{commentId}")]
        public async Task<IActionResult> DeleteCommentAsync([FromRoute] Guid eventId, [FromRoute] Guid commentId, CancellationToken token)
        {
            var userId = User.GetUserIdOrThrow();
            var isAdmin = User.IsAdminOrThrow();

            var command = new EventCommentDeleteCommand
            {
                CurrentUserId = userId,
                IsCurrentUserAdmin = isAdmin,
                EventId = eventId,
                CommentId = commentId
            };

            await _mediator.Send(command, token);

            return NoContent();
        }

        [HttpGet("/api/events/{eventId}/comments")]
        public async Task<IActionResult> GetPagedCommentsAsync([FromRoute] Guid eventId, [FromQuery] int pageNumber = 1, CancellationToken cancellationToken = default)
        {
            var pageParameters = new PageParameters { PageNumber = pageNumber, PageSize = PageSize };
            var command = new EventCommentGetPagedByEventQuery { EventId = eventId, PageParameters = pageParameters };

            var page = await _mediator.Send(command, cancellationToken);

            return Ok(page);
        }
    }
}
