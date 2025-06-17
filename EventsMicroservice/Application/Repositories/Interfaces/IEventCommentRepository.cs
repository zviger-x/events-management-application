using Domain.Entities;
using Shared.Common;
using Shared.Repositories.Interfaces;

namespace Application.Repositories.Interfaces
{
    public interface IEventCommentRepository : IRepository<EventComment>
    {
        /// <summary>
        /// Returns an array of comments.
        /// </summary>
        /// <param name="eventId">The event ID to which the comments belong.</param>
        /// <param name="pageNumber">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="token">Cancellation token to cancel the operation if needed.</param>
        /// <returns>An a paged collection of comments.</returns>
        Task<PagedCollection<EventComment>> GetPagedByEventAsync(Guid eventId, int pageNumber, int pageSize, CancellationToken token = default);
    }
}
