using DataAccess.Repositories.Interfaces;

namespace DataAccess.UnitOfWork.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository UserRepository { get; }
        IUserNotificationRepository UserNotificationRepository { get; }
        IUserTransactionRepository UserTransactionRepository { get; }

        /// <summary>
        /// Saves changes to the database.
        /// </summary>
        Task SaveChangesAsync();

        /// <summary>
        /// Begins a new transaction.
        /// </summary>
        Task BeginTransactionAsync();

        /// <summary>
        /// Saving all changes to the database and commits the current transaction.
        /// </summary>
        Task CommitTransactionAsync();

        /// <summary>
        /// Rolls back the current transaction, discarding any unsaved changes.
        /// </summary>
        Task RollbackTransactionAsync();

        /// <summary>
        /// Executes an action within a transaction.
        /// </summary>
        /// <param name="action">The action to execute within the transaction.</param>
        Task InvokeWithTransactionAsync(Func<Task> action);
    }
}
