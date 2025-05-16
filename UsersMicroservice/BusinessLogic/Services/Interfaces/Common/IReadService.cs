using DataAccess.Common;

namespace BusinessLogic.Services.Interfaces.Common
{
    public interface IReadService<TReadDto> : IDisposable
    {
        /// <summary>
        /// Gets a DTO representation of the entity with the specified ID.
        /// </summary>
        /// <param name="id">ID of the entity to retrieve.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>The DTO representing the entity.</returns>
        Task<TReadDto> GetByIdAsync(Guid id, CancellationToken token = default);

        /// <summary>
        /// Gets a list of all entity DTOs.
        /// </summary>
        /// <param name="token">Cancellation token.</param>
        /// <returns>List of DTOs representing all entities.</returns>
        Task<IEnumerable<TReadDto>> GetAllAsync(CancellationToken token = default);

        /// <summary>
        /// Gets a paged collection of entity DTOs.
        /// </summary>
        /// <param name="pageParameters">Page number and size.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>Paged collection of DTOs.</returns>
        Task<PagedCollection<TReadDto>> GetPagedAsync(PageParameters pageParameters, CancellationToken token = default);
    }
}
