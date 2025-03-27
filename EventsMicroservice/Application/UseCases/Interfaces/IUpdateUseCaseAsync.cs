using Domain.Entities.Interfaces;

namespace Application.UseCases.Interfaces
{
    /// <summary>
    /// Updates an entity.
    /// </summary>
    /// <typeparam name="T">Entity to update.</typeparam>
    public interface IUpdateUseCaseAsync<T> : IUseCase<T, CancellationToken, Task>
        where T : class, IEntity
    {
    }
}
