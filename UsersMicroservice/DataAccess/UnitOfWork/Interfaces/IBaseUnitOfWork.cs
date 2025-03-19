using DataAccess.Entities.Interfaces;
using DataAccess.Repositories.Interfaces;

namespace DataAccess.UnitOfWork.Interfaces
{
    public interface IBaseUnitOfWork : IDisposable
    {
        IRepository<T> Repository<T>() where T : class, IEntity;

        /// <summary>
        /// Saves changes to the database.
        /// </summary>
        Task SaveChangesAsync(CancellationToken token = default);

        /// <summary>
        /// Begins a new transaction.
        /// </summary>
        Task BeginTransactionAsync(CancellationToken token = default);

        /// <summary>
        /// Saving all changes to the database and commits the current transaction.
        /// </summary>
        Task CommitTransactionAsync(CancellationToken token = default);

        /// <summary>
        /// Rolls back the current transaction, discarding any unsaved changes.
        /// </summary>
        Task RollbackTransactionAsync(CancellationToken token = default);

        /// <summary>
        /// Executes an action within a transaction.
        /// Saving all changes to the database and commits the current transaction.
        /// </summary>
        /// <param name="action">The action to execute within the transaction.</param>
        Task InvokeWithTransactionAsync(Func<CancellationToken, Task> action, CancellationToken token = default);
    }
}
