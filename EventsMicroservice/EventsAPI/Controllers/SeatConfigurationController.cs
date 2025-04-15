using Application.Contracts;
using Application.MediatR.Commands.SeatConfigurationCommands;
using Application.MediatR.Queries.SeatConfigurationQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Attributes;
using Shared.Common;
using Shared.Enums;

namespace EventsAPI.Controllers
{
    [AuthorizeRoles(UserRoles.EventManager, UserRoles.Admin)]
    [ApiController]
    [Route("api/seat-configurations")]
    public class SeatConfigurationController : Controller
    {
        private const int PageSize = 10;

        private readonly IMediator _mediator;

        public SeatConfigurationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateSeatConfigurationDto seatConfigurationToCreate, CancellationToken cancellationToken)
        {
            var command = new SeatConfigurationCreateCommand { SeatConfiguration = seatConfigurationToCreate };
            var createdConfigurationId = await _mediator.Send(command, cancellationToken);

            return StatusCode(StatusCodes.Status201Created, new { Id = createdConfigurationId });
        }

        [HttpPut("{seatId}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid seatId, [FromBody] UpdateSeatConfigurationDto seatConfigurationToUpdate, CancellationToken cancellationToken)
        {
            var command = new SeatConfigurationUpdateCommand { RouteSeatId = seatId, SeatConfiguration = seatConfigurationToUpdate };
            await _mediator.Send(command, cancellationToken);

            return Ok();
        }

        [HttpDelete("{seatId}")]
        public async Task<IActionResult> DeleteAsync(Guid seatId, CancellationToken cancellationToken)
        {
            var command = new SeatConfigurationDeleteCommand { Id = seatId };
            await _mediator.Send(command, cancellationToken);

            return NoContent();
        }

        [HttpGet("{seatId}")]
        public async Task<IActionResult> GetByIdAsync(Guid seatId, CancellationToken cancellationToken)
        {
            var query = new SeatConfigurationGetByIdQuery { Id = seatId };
            var configuration = await _mediator.Send(query, cancellationToken);

            return Ok(configuration);
        }

        [HttpGet]
        public async Task<IActionResult> GetPagedAsync([FromQuery] int pageNumber = 1, CancellationToken cancellationToken = default)
        {
            var pageParameters = new PageParameters { PageNumber = pageNumber, PageSize = PageSize };
            var query = new SeatConfigurationGetPagedQuery { PageParameters = pageParameters };

            var page = await _mediator.Send(query, cancellationToken);

            return Ok(page);
        }
    }
}
