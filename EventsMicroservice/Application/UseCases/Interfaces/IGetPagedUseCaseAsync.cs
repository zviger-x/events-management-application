using Domain.Entities;
using Domain.Entities.Interfaces;

namespace Application.UseCases.Interfaces
{
    /// <summary>
    /// Returns an array of entities
    /// </summary>
    /// <typeparam name="T">The type of the entity to retrieve.</typeparam>
    public interface IGetPagedUseCaseAsync<T> : IUseCase<CancellationToken, Task<PagedCollection<T>>>
        where T : IEntity
    {
    }
}
