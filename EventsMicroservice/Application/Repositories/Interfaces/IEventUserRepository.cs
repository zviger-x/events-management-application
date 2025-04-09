using Domain.Entities;
using Shared.Common;
using Shared.Repositories.Interfaces;

namespace Application.Repositories.Interfaces
{
    public interface IEventUserRepository : IRepository<EventUser>
    {
        /// <summary>
        /// Returns an array of users.
        /// </summary>
        /// <param name="eventId">The event ID to which the users belong.</param>
        /// <param name="pageNumber">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="token">Cancellation token to cancel the operation if needed.</param>
        /// <returns>An a paged collection of users.</returns>
        Task<PagedCollection<EventUser>> GetPagedByEventAsync(Guid eventId, int pageNumber, int pageSize, CancellationToken token = default);
    }
}
