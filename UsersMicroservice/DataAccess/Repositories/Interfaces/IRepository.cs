using DataAccess.Entities.Interfaces;

namespace DataAccess.Repositories.Interfaces
{
    public interface IRepository<T> : IDisposable
        where T : IEntity
    {
        /// <summary>
        /// Creates an entity.
        /// </summary>
        /// <param name="entity">Entity to create.</param>
        Task CreateAsync(T entity);

        /// <summary>
        /// Updates an entity.
        /// </summary>
        /// <param name="entity">Entity to update.</param>
        Task UpdateAsync(T entity);

        /// <summary>
        /// Removes an entity.
        /// </summary>
        /// <param name="id">Id of the entity to delete.</param>
        Task DeleteAsync(int id);

        /// <summary>
        /// Returns an entity by its id.
        /// </summary>
        /// <param name="id">Entity id.</param>
        /// <returns>Entity.</returns>
        Task<T> GetByIdAsync(int id);

        /// <summary>
        /// Returns an array of all entities.
        /// </summary>
        /// <returns>An array of all entities</returns>
        IQueryable<T> GetAll();

        /// <summary>
        /// Saves all changes made in this context to the database.
        /// </summary>
        Task SaveChangesAsync();
    }
}
