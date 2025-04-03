using Application.MediatR.Commands.SeatConfigurationCommands;
using Application.MediatR.Queries.SeatConfigurationQueries;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EventsAPI.Controllers
{
    [ApiController]
    [Route("api/seat-configurations")]
    public class SeatConfigurationController : Controller
    {
        private readonly IMediator _mediator;

        private const int PageSize = 10;

        public SeatConfigurationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] SeatConfiguration seatConfigurationToCreate, CancellationToken cancellationToken)
        {
            var command = new SeatConfigurationCreateCommand { SeatConfiguration = seatConfigurationToCreate };
            var createdConfigurationId = await _mediator.Send(command, cancellationToken);

            return Ok(createdConfigurationId);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] SeatConfiguration seatConfigurationToUpdate, CancellationToken cancellationToken)
        {
            if (id != seatConfigurationToUpdate.Id)
                throw new ArgumentException("You are not allowed to modify this configuration.");

            var command = new SeatConfigurationUpdateCommand { SeatConfiguration = seatConfigurationToUpdate };
            await _mediator.Send(command, cancellationToken);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var command = new SeatConfigurationDeleteCommand { Id = id };
            await _mediator.Send(command, cancellationToken);

            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var query = new SeatConfigurationGetByIdQuery { Id = id };
            var configuration = await _mediator.Send(query, cancellationToken);

            if (configuration == null)
                return NotFound();

            return Ok(configuration);
        }

        [HttpGet]
        public async Task<IActionResult> GetPagedAsync([FromQuery] int pageNumber = 1, CancellationToken cancellationToken = default)
        {
            var query = new SeatConfigurationGetPagedQuery { PageNumber = pageNumber, PageSize = PageSize };
            var page = await _mediator.Send(query, cancellationToken);

            return Ok(page);
        }
    }
}
