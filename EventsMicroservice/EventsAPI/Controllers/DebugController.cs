using Application.MediatR.Queries.EventCommentQueries;
using Application.UnitOfWork.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Attributes;
using Shared.Common;
using Shared.Enums;

namespace EventsAPI.Controllers
{
#warning TODO: Удалить контроллер
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
            var pageParameters = new PageParameters { PageNumber = 1, PageSize = 100 };
            var query = new EventCommentGetPagedQuery { PageParameters = pageParameters };

            var page = await _mediator.Send(query, cancellationToken);

            return Ok(page);
        }

        [HttpGet("seats")]
        public async Task<IActionResult> GetAllSeatsAsync(CancellationToken cancellationToken = default)
        {
            var seats = await _unitOfWork.SeatRepository.GetAllAsync(cancellationToken);

            return Ok(seats);
        }

        [Authorize]
        [HttpGet("authenticate")]
        public IActionResult AuthenticateAll() => Ok();

        [AuthorizeRoles(UserRoles.EventManager, UserRoles.Admin)]
        [HttpGet("authenticate-event-manager")]
        public IActionResult AuthenticateEventManager() => Ok();

        [AuthorizeRoles(UserRoles.Admin)]
        [HttpGet("authenticate-admin")]
        public IActionResult AuthenticateAdmin() => Ok();
    }
}
