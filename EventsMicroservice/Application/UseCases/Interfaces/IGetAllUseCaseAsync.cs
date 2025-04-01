using Domain.Entities.Interfaces;

namespace Application.UseCases.Interfaces
{
    /// <summary>
    /// Returns an array of all entities
    /// </summary>
    /// <typeparam name="T">The type of the entity to retrieve.</typeparam>
    public interface IGetAllUseCaseAsync<T> : IUseCase<CancellationToken, Task<IEnumerable<T>>>
        where T : class
    {
    }
}
