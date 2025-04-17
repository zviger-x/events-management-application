using DataAccess.Common;
using DataAccess.Entities.Interfaces;

namespace BusinessLogic.Services.Interfaces
{
    public interface IService<T> : IDisposable
        where T : class, IEntity
    {
        /// <summary>
        /// Creates an entity.
        /// </summary>
        /// <param name="entity">Entity to create.</param>
        /// <param name="token">Cancellation token to cancel the operation if needed.</param>
        /// <returns>A response containing validation errors, if any.</returns>
        Task CreateAsync(T entity, CancellationToken token = default);

        /// <summary>
        /// Updates an entity.
        /// </summary>
        /// <param name="routeEntityId">Entity id from route.</param>
        /// <param name="entity">Entity to update.</param>
        /// <param name="token">Cancellation token to cancel the operation if needed.</param>
        /// <returns>A response containing validation errors, if any.</returns>
        Task UpdateAsync(Guid routeEntityId, T entity, CancellationToken token = default);

        /// <summary>
        /// Removes an entity.
        /// </summary>
        /// <param name="id">Id of the entity to delete.</param>
        /// <param name="token">Cancellation token to cancel the operation if needed.</param>
        /// <returns>A response containing validation errors, if any.</returns>
        Task DeleteAsync(Guid id, CancellationToken token = default);

        /// <summary>
        /// Returns an entity by its id.
        /// </summary>
        /// <param name="id">Entity id.</param>
        /// <param name="token">Cancellation token to cancel the operation if needed.</param>
        /// <returns>A response with validation errors, if any, or the entity.</returns>
        Task<T> GetByIdAsync(Guid id, CancellationToken token = default);

        /// <summary>
        /// Returns an array of all entities.
        /// </summary>
        /// <param name="token">Cancellation token to cancel the operation if needed.</param>
        /// <returns>A response with validation errors, if any, or a collection of entities</returns>
        Task<IEnumerable<T>> GetAllAsync(CancellationToken token = default);

        /// <summary>
        /// Returns an array of entities.
        /// </summary>
        /// <param name="pageNumber">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="token">Cancellation token to cancel the operation if needed.</param>
        /// <returns>An a paged collection of entities</returns>
        Task<PagedCollection<T>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken token = default);
    }
}
