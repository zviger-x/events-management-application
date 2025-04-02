namespace Application.UseCases.Interfaces
{
    /// <summary>
    /// Removes an entity.
    /// </summary>
    public interface IDeleteUseCaseAsync<T> : IUseCase<Guid, CancellationToken, Task>
        where T : class
    {
    }
}
