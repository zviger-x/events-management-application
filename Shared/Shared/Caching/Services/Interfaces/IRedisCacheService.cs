using Shared.Caching.Locks.Interfaces;

namespace Shared.Caching.Services.Interfaces
{
    public interface IRedisCacheService : ICacheService
    {
        /// <summary>
        /// Asynchronously attempts to acquire a lock on a specified key in Redis.
        /// The lock will be associated with the given value and will have a time-to-live (TTL).
        /// If the lock is successfully acquired, an <see cref="IRedisLock"/> instance will be returned.
        /// The operation can be canceled via the provided <paramref name="cancellationToken"/>.
        /// </summary>
        /// <param name="lockKey">The key representing the resource to lock in Redis.</param>
        /// <param name="lockValue">The unique value associated with the lock, typically a GUID or token to identify the lock holder.</param>
        /// <param name="lockTtl">The time-to-live (TTL) of the lock, specifying how long the lock should remain active in Redis.</param>
        /// <param name="cancellationToken">An optional <see cref="CancellationToken"/> that can be used to cancel the lock acquisition operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is an <see cref="IRedisLock"/> instance if the lock is successfully acquired; otherwise, <see langword="null"/>.</returns>
        Task<IRedisLock> AcquireLockAsync(string lockKey, string lockValue, TimeSpan lockTtl, CancellationToken cancellationToken = default);
    }
}
