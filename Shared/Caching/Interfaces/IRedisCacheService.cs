namespace Shared.Caching.Interfaces
{
    public interface IRedisCacheService : ICacheService
    {
        /// <summary>
        /// Attempts to acquire a distributed lock by setting a key with a specified TTL.
        /// </summary>
        /// <param name="lockKey">The key used to identify the lock in the cache.</param>
        /// <param name="lockValue">The value to associate with the lock key.</param>
        /// <param name="lockTtl">The time-to-live duration for the lock.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation if needed.</param>
        /// <returns>True if the lock was successfully acquired; otherwise, false.</returns>
        Task<bool> AcquireLockAsync(string lockKey, string lockValue, TimeSpan lockTtl, CancellationToken cancellationToken = default);

        /// <summary>
        /// Releases the previously acquired distributed lock by removing the key from the cache.
        /// </summary>
        /// <param name="lockKey">The key used to identify the lock in the cache.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation if needed.</param>
        Task ReleaseLockAsync(string lockKey, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates the TTL (time-to-live) of an existing key in the cache.
        /// </summary>
        /// <param name="key">The key whose TTL should be refreshed.</param>
        /// <param name="ttl">The new TTL to apply to the key.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation if needed.</param>
        /// <returns>True if the TTL was successfully updated; otherwise, false.</returns>
        Task<bool> RefreshKeyTtlAsync(string key, TimeSpan ttl, CancellationToken cancellationToken = default);
    }
}
