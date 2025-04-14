using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Caching.Interfaces;
using Shared.Configuration;
using System.Text.Json;

namespace Shared.Caching
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<RedisCacheService> _logger;

        private readonly TimeSpan _defaultExpirationTime;

        public RedisCacheService(IDistributedCache cache, ILogger<RedisCacheService> logger, IOptions<CacheConfig> cacheConfig)
        {
            _cache = cache;
            _logger = logger;

            var config = cacheConfig.Value;
            if (config.CacheExpirationSeconds <= 0)
                throw new InvalidOperationException("Invalid cache expiration time configuration.");

            _defaultExpirationTime = TimeSpan.FromSeconds(config.CacheExpirationSeconds);
        }

        public async Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            var cachedData = await _cache.GetStringAsync(key, cancellationToken);
            if (string.IsNullOrEmpty(cachedData)) return default;

            _logger.LogInformation($"Cache hit for key: {key}");
            return JsonSerializer.Deserialize<T>(cachedData);
        }

        public async Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default)
        {
            await SetAsync(key, value, _defaultExpirationTime, cancellationToken);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan expirationTime, CancellationToken cancellationToken = default)
        {
            var options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(expirationTime);
            var serializedData = JsonSerializer.Serialize(value);
            await _cache.SetStringAsync(key, serializedData, options, cancellationToken);

            _logger.LogInformation($"Cache set for key: {key} with expiration: {expirationTime.TotalSeconds} seconds");
        }

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> getDataAsyncFunc, CancellationToken cancellationToken = default)
        {
            return await GetOrSetAsync(key, getDataAsyncFunc, _defaultExpirationTime, cancellationToken);
        }

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> getDataAsyncFunc, TimeSpan expirationTime, CancellationToken cancellationToken = default)
        {
            var cachedData = await GetAsync<T>(key, cancellationToken);
            if (cachedData != null)
                return cachedData;

            var fetchedData = await getDataAsyncFunc();
            if (fetchedData != null)
                await SetAsync(key, fetchedData, expirationTime, cancellationToken);

            return fetchedData;
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            await _cache.RemoveAsync(key, cancellationToken);
            _logger.LogInformation($"Cache removed for key: {key}");
        }
    }
}
