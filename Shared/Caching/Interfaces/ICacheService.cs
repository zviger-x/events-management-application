using StackExchange.Redis;

namespace Shared.Caching.Interfaces
{
    public interface ICacheService
    {
        /// <summary>
        /// Retrieves a value from the cache by its key.
        /// </summary>
        /// <typeparam name="T">The type of the cached value.</typeparam>
        /// <param name="key">The key associated with the cached value.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation if needed.</param>
        /// <returns>The cached value, or null if the key doesn't exist.</returns>
        Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets a value in the cache with a specified key and expiration time.
        /// </summary>
        /// <typeparam name="T">The type of the value to be cached.</typeparam>
        /// <param name="key">The key to associate with the cached value.</param>
        /// <param name="value">The value to be cached.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation if needed.</param>
        Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets a value in the cache with a specified key and expiration time.
        /// </summary>
        /// <typeparam name="T">The type of the value to be cached.</typeparam>
        /// <param name="key">The key to associate with the cached value.</param>
        /// <param name="value">The value to be cached.</param>
        /// <param name="expirationTime">Expiration time.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation if needed.</param>
        Task SetAsync<T>(string key, T value, TimeSpan expirationTime, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a value from the cache by its key, or fetches and stores it using the provided delegate if not found.
        /// </summary>
        /// <typeparam name="T">The type of the value to retrieve or fetch.</typeparam>
        /// <param name="key">The key associated with the cached value.</param>
        /// <param name="getDataAsyncFunc">A function to get the data if it is not found in the cache.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation if needed.</param>
        /// <returns>
        /// The cached or fetched value. Returns <see langword="null"/> if the value could not be retrieved or fetched; 
        /// in this case, the value will not be cached.
        /// </returns>
        Task<T> GetOrSetAsync<T>(string key, Func<CancellationToken, Task<T>> getDataAsyncFunc, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a value from the cache by its key, or fetches and stores it using the provided delegate if not found.
        /// </summary>
        /// <typeparam name="T">The type of the value to retrieve or fetch.</typeparam>
        /// <param name="key">The key associated with the cached value.</param>
        /// <param name="getDataAsyncFunc">A function to get the data if it is not found in the cache.</param>
        /// <param name="expirationTime">The duration for which the data should be stored in the cache.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation if needed.</param>
        /// <returns>
        /// The cached or fetched value. Returns <see langword="null"/> if the value could not be retrieved or fetched; 
        /// in this case, the value will not be cached.
        /// </returns>
        Task<T> GetOrSetAsync<T>(string key, Func<CancellationToken, Task<T>> getDataAsyncFunc, TimeSpan expirationTime, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes a value from the cache by its key.
        /// </summary>
        /// <param name="key">The key associated with the cached value to be removed.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation if needed.</param>
        Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    }

    public interface IRedisCacheService : ICacheService
    {
        Task<bool> AcquireLockAsync(string lockKey, string lockValue, TimeSpan lockTtl, CancellationToken cancellationToken = default);

        Task ReleaseLockAsync(string lockKey, CancellationToken cancellationToken = default);

        Task<bool> RefreshKeyTtlAsync(string key, TimeSpan ttl, CancellationToken cancellationToken = default);
    }
}
