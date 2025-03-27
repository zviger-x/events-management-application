using Application.UseCases.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EventsAPI.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventController : Controller
    {
        private readonly ICreateUseCaseAsync<Event> _createUseCaseAsync;
        private readonly IUpdateUseCaseAsync<Event> _updateUseCaseAsync;
        private readonly IDeleteUseCaseAsync<Event> _deleteUseCaseAsync;
        private readonly IGetByIdUseCaseAsync<Event> _getByIdUseCaseAsync;
        private readonly IGetAllUseCaseAsync<Event> _getAllUseCasesAsync;
        private readonly IGetPagedUseCaseAsync<Event> _getPagedUseCasesAsync;

        private const int PageSize = 10;

        public EventController(ICreateUseCaseAsync<Event> createUseCaseAsync,
            IUpdateUseCaseAsync<Event> updateUseCaseAsync,
            IDeleteUseCaseAsync<Event> deleteUseCaseAsync,
            IGetByIdUseCaseAsync<Event> getByIdUseCaseAsync,
            IGetAllUseCaseAsync<Event> getAllUseCasesAsync,
            IGetPagedUseCaseAsync<Event> getPagedUseCasesAsync)
        {
            _createUseCaseAsync = createUseCaseAsync;
            _updateUseCaseAsync = updateUseCaseAsync;
            _deleteUseCaseAsync = deleteUseCaseAsync;
            _getByIdUseCaseAsync = getByIdUseCaseAsync;
            _getAllUseCasesAsync = getAllUseCasesAsync;
            _getPagedUseCasesAsync = getPagedUseCasesAsync;
        }


        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] Event eventToCreate, CancellationToken cancellationToken)
        {
            await _createUseCaseAsync.Execute(eventToCreate, cancellationToken);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] Event eventToUpdate, CancellationToken cancellationToken)
        {
            if (id != eventToUpdate.Id)
                throw new ArgumentException("You are not allowed to modify this event.");

            await _updateUseCaseAsync.Execute(eventToUpdate, cancellationToken);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            await _deleteUseCaseAsync.Execute(id, cancellationToken);

            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var @event = await _getByIdUseCaseAsync.Execute(id, cancellationToken);

            return Ok(@event);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
        {
            var collection = await _getAllUseCasesAsync.Execute(cancellationToken);

            return Ok(collection);
        }

        [HttpGet]
        public async Task<IActionResult> GetPagedAsync([FromQuery] int pageNumber = 1, CancellationToken cancellationToken)
        {
            var page = await _getPagedUseCasesAsync.Execute(pageNumber, PageSize, cancellationToken);

            return Ok(page);
        }
    }
}
