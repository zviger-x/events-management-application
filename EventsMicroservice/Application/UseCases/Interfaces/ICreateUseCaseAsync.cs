using Domain.Entities.Interfaces;

namespace Application.UseCases.Interfaces
{
    /// <summary>
    /// Creates an entity.
    /// </summary>
    /// <typeparam name="T">Entity to create.</typeparam>
    public interface ICreateUseCaseAsync<T> : IUseCase<T, CancellationToken, Task>
        where T : class, IEntity
    {
    }
}
