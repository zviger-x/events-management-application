namespace BusinessLogic.Services.Interfaces.Common
{
    public interface IDeleteService : IDisposable
    {
        /// <summary>
        /// Deletes the entity with the specified ID.
        /// </summary>
        /// <param name="id">ID of the entity to delete.</param>
        /// <param name="token">Cancellation token.</param>
        Task DeleteAsync(Guid id, CancellationToken token = default);
    }
}
