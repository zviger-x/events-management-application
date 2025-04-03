using Domain.Entities;

namespace Application.Repositories.Interfaces
{
    public interface IReviewRepository : IRepository<Review>
    {
        /// <summary>
        /// Returns an array of entities.
        /// </summary>
        /// <param name="eventId">Event Id.</param>
        /// <param name="pageNumber">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="token">Cancellation token to cancel the operation if needed.</param>
        /// <returns>An a paged collection of entities</returns>
        Task<PagedCollection<Review>> GetPagedByEventAsync(Guid eventId, int pageNumber, int pageSize, CancellationToken token = default);
    }
}
