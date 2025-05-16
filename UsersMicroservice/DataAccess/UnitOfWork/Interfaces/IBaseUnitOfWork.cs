namespace DataAccess.UnitOfWork.Interfaces
{
    public interface IBaseUnitOfWork : IDisposable
    {
        /// <summary>
        /// Begins a new transaction.
        /// </summary>
        /// <param name="token">Cancellation token to cancel the operation if needed.</param>
        Task BeginTransactionAsync(CancellationToken token = default);

        /// <summary>
        /// Saving all changes to the database and commits the current transaction.
        /// </summary>
        /// <param name="token">Cancellation token to cancel the operation if needed.</param>
        Task CommitTransactionAsync(CancellationToken token = default);

        /// <summary>
        /// Rolls back the current transaction, discarding any unsaved changes.
        /// </summary>
        /// <param name="token">Cancellation token to cancel the operation if needed.</param>
        Task RollbackTransactionAsync(CancellationToken token = default);

        /// <summary>
        /// Executes an action within a transaction.
        /// Saving all changes to the database and commits the current transaction.
        /// </summary>
        /// <param name="action">The action to execute within the transaction.</param>
        /// <param name="token">Cancellation token to cancel the operation if needed.</param>
        Task InvokeWithTransactionAsync(Func<CancellationToken, Task> action, CancellationToken token = default);
    }
}
