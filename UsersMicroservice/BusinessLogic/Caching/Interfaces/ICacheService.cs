namespace BusinessLogic.Caching.Interfaces
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
        Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets a value in the cache with a specified key and expiration time.
        /// </summary>
        /// <typeparam name="T">The type of the value to be cached.</typeparam>
        /// <param name="key">The key to associate with the cached value.</param>
        /// <param name="value">The value to be cached.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation if needed.</param>
        Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes a value from the cache by its key.
        /// </summary>
        /// <param name="key">The key associated with the cached value to be removed.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation if needed.</param>
        Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    }
}
