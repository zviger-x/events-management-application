using Domain.Entities.Interfaces;

namespace Application.UseCases.Interfaces
{
    /// <summary>
    /// Removes an entity.
    /// </summary>
    /// <typeparam name="T">Entity to delete.</typeparam>
    public interface IDeleteUseCaseAsync<T> : IUseCase<T, CancellationToken, Task>
        where T : IEntity
    {
    }
}
