namespace Shared.Caching.Locks.Interfaces
{
    /// <summary>
    /// Represents a Redis-based lock that ensures exclusive access to a resource.
    /// Implements IAsyncDisposable to release the lock asynchronously when no longer needed.
    /// </summary>
    public interface IRedisLock : IAsyncDisposable
    {
        /// <summary>
        /// The key associated with the lock.
        /// </summary>
        string Key { get; }
    }
}
