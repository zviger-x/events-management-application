namespace BusinessLogic.Services.Interfaces.Common
{
    public interface IUpdateService<TUpdateDto> : IDisposable
    {
        /// <summary>
        /// Updates an existing entity identified by ID with the provided DTO.
        /// </summary>
        /// <param name="id">ID of the entity to update.</param>
        /// <param name="dto">DTO containing the updated data.</param>
        /// <param name="token">Cancellation token.</param>
        Task UpdateAsync(Guid id, TUpdateDto dto, CancellationToken token = default);
    }
}
