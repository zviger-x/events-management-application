using Application.Specifications.Interfaces;
using Domain.Entities;
using Shared.Common;
using Shared.Repositories.Interfaces;

namespace Application.Repositories.Interfaces
{
    public interface IEventRepository : IRepository<Event>
    {
        /// <summary>
        /// Retrieves a paged collection of events that satisfy the given specification criteria.
        /// </summary>
        /// <param name="specification">The specification defining the filtering criteria for events.</param>
        /// <param name="pageNumber">The page number to retrieve.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>A paged collection of events matching the specification.</returns>
        Task<PagedCollection<Event>> GetPagedByFilterAsync(ISpecification<Event> specification, int pageNumber, int pageSize, CancellationToken token = default);
    }
}
