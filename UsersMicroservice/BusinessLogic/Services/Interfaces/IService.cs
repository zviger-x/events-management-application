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
        /// <returns>A response containing validation errors, if any.</returns>
        Task CreateAsync(T entity, CancellationToken token = default);

        /// <summary>
        /// Updates an entity.
        /// </summary>
        /// <param name="entity">Entity to update.</param>
        /// <returns>A response containing validation errors, if any.</returns>
        Task UpdateAsync(T entity, CancellationToken token = default);

        /// <summary>
        /// Removes an entity.
        /// </summary>
        /// <param name="id">Id of the entity to delete.</param>
        /// <returns>A response containing validation errors, if any.</returns>
        Task DeleteAsync(Guid id, CancellationToken token = default);

        /// <summary>
        /// Returns an entity by its id.
        /// </summary>
        /// <param name="id">Entity id.</param>
        /// <returns>A response with validation errors, if any, or the entity.</returns>
        Task<T> GetByIdAsync(Guid id, CancellationToken token = default);

        /// <summary>
        /// Returns an array of all entities.
        /// </summary>
        /// <returns>A response with validation errors, if any, or a collection of entities</returns>
        IEnumerable<T> GetAll();

        /// <summary>
        /// Saves all changes made in this context to the database.
        /// </summary>
        Task SaveChangesAsync(CancellationToken token = default);
    }
}
