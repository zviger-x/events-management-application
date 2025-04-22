namespace BusinessLogic.Services.Interfaces.Common
{
    public interface ICreateService<TCreateDto> : IDisposable
    {
        /// <summary>
        /// Creates a new entity based on the given DTO.
        /// </summary>
        /// <param name="createDto">DTO containing the data for creation.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>Identifier of the created entity.</returns>
        Task<Guid> CreateAsync(TCreateDto createDto, CancellationToken token = default);
    }
}
