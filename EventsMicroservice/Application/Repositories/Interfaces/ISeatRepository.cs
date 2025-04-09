using Domain.Entities;
using Shared.Repositories.Interfaces;

namespace Application.Repositories.Interfaces
{
    public interface ISeatRepository : IRepository<Seat>
    {
        /// <summary>
        /// Returns an array of seats.
        /// </summary>
        /// <param name="eventId">The event ID to which the seats belong.</param>
        /// <param name="token">Cancellation token to cancel the operation if needed.</param>
        /// <returns>An a collection of seats.</returns>
        Task<IEnumerable<Seat>> GetByEventAsync(Guid eventId, CancellationToken cancellationToken = default);
    }
}
