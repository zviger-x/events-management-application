using DataAccess.Entities;
using DataAccess.Entities.Interfaces;

namespace DataAccess.Repositories.Interfaces
{
    public interface IRepository<T> : IDisposable
        where T : class, IEntity
    {
        /// <summary>
        /// Creates an entity.
        /// </summary>
        /// <param name="entity">Entity to create.</param>
        Task CreateAsync(T entity, CancellationToken token = default);

        /// <summary>
        /// Updates an entity.
        /// </summary>
        /// <param name="entity">Entity to update.</param>
        Task UpdateAsync(T entity, CancellationToken token = default);

        /// <summary>
        /// Removes an entity.
        /// </summary>
        /// <param name="entity">Entity to delete.</param>
        Task DeleteAsync(T entity, CancellationToken token = default);

        /// <summary>
        /// Returns an entity by its id.
        /// </summary>
        /// <param name="id">Entity id.</param>
        /// <returns>Entity.</returns>
        Task<T> GetByIdAsync(Guid id, CancellationToken token = default);

        /// <summary>
        /// Returns an array of all entities.
        /// </summary>
        /// <returns>An array of all entities</returns>
        Task<IEnumerable<T>> GetAllAsync(CancellationToken token = default);

        /// <summary>
        /// Returns an array of entities.
        /// </summary>
        /// <param name="pageNumber">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        /// <returns>An a paged collection of entities</returns>
        Task<PagedCollection<T>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken token = default);

        /// <summary>
        /// Saves all changes made in this context to the database.
        /// </summary>
        Task SaveChangesAsync(CancellationToken token = default);
    }
}
