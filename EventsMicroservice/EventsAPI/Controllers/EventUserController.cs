using Application.MediatR.Commands.EventUserCommands;
using Application.MediatR.Queries.EventUserQueries;
using EventsAPI.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Attributes;
using Shared.Common;
using Shared.Enums;

namespace EventsAPI.Controllers
{
    [ApiController]
    [Route("api/event-users")]
    public class EventUserController : Controller
    {
        private const int PageSize = 10;

        private readonly IMediator _mediator;

        public EventUserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [HttpPost("/api/events/{eventId}/seats/{seatId}/register")]
        public async Task<IActionResult> RegisterUser([FromRoute] Guid eventId, [FromRoute] Guid seatId, CancellationToken cancellationToken)
        {
            var userId = User.GetUserIdOrThrow();
            var command = new EventUserCreateCommand { EventId = eventId, UserId = userId, SeatId = seatId };

            var registeredUserId = await _mediator.Send(command, cancellationToken);

            return Ok(registeredUserId);
        }

        [AuthorizeRoles(UserRoles.Admin)]
        [HttpDelete("{eventUserId}")]
        public async Task<IActionResult> UnregisterUser([FromRoute] Guid eventUserId, CancellationToken cancellationToken)
        {
            var command = new EventUserDeleteCommand { Id = eventUserId };

            await _mediator.Send(command, cancellationToken);

            return Ok();
        }

        [AuthorizeRoles(UserRoles.EventManager, UserRoles.Admin)]
        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, CancellationToken cancellationToken = default)
        {
            var pageParameters = new PageParameters { PageNumber = pageNumber, PageSize = PageSize };
            var query = new EventUserGetPagedQuery { PageParameters = pageParameters };

            var page = await _mediator.Send(query, cancellationToken);

            return Ok(page);
        }

        [AuthorizeRoles(UserRoles.EventManager, UserRoles.Admin)]
        [HttpGet("/api/events/{eventId}/users")]
        public async Task<IActionResult> GetPagedByEvent([FromRoute] Guid eventId, [FromQuery] int pageNumber = 1, CancellationToken cancellationToken = default)
        {
            var pageParameters = new PageParameters { PageNumber = pageNumber, PageSize = PageSize };
            var query = new EventUserGetPagedByEventQuery { EventId = eventId, PageParameters = pageParameters };

            var page = await _mediator.Send(query, cancellationToken);

            return Ok(page);
        }
    }
}
