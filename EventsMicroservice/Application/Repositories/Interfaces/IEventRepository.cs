using Domain.Entities;
using Shared.Common;
using Shared.Repositories.Interfaces;

namespace Application.Repositories.Interfaces
{
    public interface IEventRepository : IRepository<Event>
    {
        /// <summary>
        /// Retrieves a paged collection of events filtered by optional name, description, location, and date range.
        /// </summary>
        /// <param name="name">Optional filter by event name (partial match).</param>
        /// <param name="description">Optional filter by event description (partial match).</param>
        /// <param name="location">Optional filter by event location (partial match).</param>
        /// <param name="fromDate">Optional filter for events starting on or after this date.</param>
        /// <param name="toDate">Optional filter for events ending on or before this date.</param>
        /// <param name="pageNumber">The page number to retrieve (1-based).</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>A paged collection of events matching the specified filters.</returns>
        Task<PagedCollection<Event>> GetPagedByFilterAsync(string name, string description, string location, DateTimeOffset? fromDate, DateTimeOffset? toDate, int pageNumber, int pageSize, CancellationToken token = default);
    }
}
