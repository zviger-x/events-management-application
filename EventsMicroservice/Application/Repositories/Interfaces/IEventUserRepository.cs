using Domain.Entities;
using Shared.Common;
using Shared.Repositories.Interfaces;

namespace Application.Repositories.Interfaces
{
    public interface IEventUserRepository : IRepository<EventUser>
    {
        /// <summary>
        /// Retrieves the event user entry for the specified user and event identifiers.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="eventId">The unique identifier of the event.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>The <see cref="EventUser"/> entity if found; otherwise, null.</returns>
        Task<EventUser> GetByUserAndEventAsync(Guid userId, Guid eventId, CancellationToken cancellationToken = default);

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
