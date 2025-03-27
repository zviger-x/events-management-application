using Domain.Entities.Interfaces;

namespace Application.UseCases.Interfaces
{
    /// <summary>
    /// Returns an entity by its id.
    /// </summary>
    /// <typeparam name="T">The type of the entity to retrieve.</typeparam>
    public interface IGetByIdUseCaseAsync<T> : IUseCase<Guid, CancellationToken, Task<T>>
        where T : IEntity
    {
    }
}
