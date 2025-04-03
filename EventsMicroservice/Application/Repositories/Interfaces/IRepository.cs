using Domain.Entities;
using Domain.Entities.Interfaces;

namespace Application.Repositories.Interfaces
{
    public interface IRepository<T> : IDisposable
        where T : class, IEntity
    {
        /// <summary>
        /// Creates an entity and automatically saves it to the database.
        /// </summary>
        /// <param name="entity">Entity to create.</param>
        /// <param name="token">Cancellation token to cancel the operation if needed.</param>
        /// <returns>The identifier of the created entity.</returns>
        Task<Guid> CreateAsync(T entity, CancellationToken token = default);

        /// <summary>
        /// Updates an entity and automatically saves the changes to the database.
        /// </summary>
        /// <param name="entity">Entity to update.</param>
        /// <param name="token">Cancellation token to cancel the operation if needed.</param>
        Task UpdateAsync(T entity, CancellationToken token = default);

        /// <summary>
        /// Removes an entity and automatically saves the changes to the database.
        /// </summary>
        /// <param name="entity">Entity to delete.</param>
        /// <param name="token">Cancellation token to cancel the operation if needed.</param>
        Task DeleteAsync(T entity, CancellationToken token = default);

        /// <summary>
        /// Returns an entity by its id.
        /// </summary>
        /// <param name="id">Entity id.</param>
        /// <param name="token">Cancellation token to cancel the operation if needed.</param>
        /// <returns>Entity.</returns>
        Task<T> GetByIdAsync(Guid id, CancellationToken token = default);

        /// <summary>
        /// Returns an array of all entities.
        /// </summary>
        /// <param name="token">Cancellation token to cancel the operation if needed.</param>
        /// <returns>An array of all entities</returns>
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
