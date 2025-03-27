using Domain.Entities.Interfaces;

namespace Application.UseCases.Interfaces
{
    /// <summary>
    /// Removes an entity.
    /// </summary>
    public interface IDeleteUseCaseAsync : IUseCase<Guid, CancellationToken, Task>
    {
    }
}
