using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Caching.Interfaces;
using Shared.Configuration;
using Shared.Logging.Extensions;
using StackExchange.Redis;
using System.Text.Json;

namespace Shared.Caching
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly IDatabase _database;
        private readonly ILogger<RedisCacheService> _logger;

        private readonly TimeSpan _defaultExpirationTime;

        public RedisCacheService(IConnectionMultiplexer connectionMultiplexer, ILogger<RedisCacheService> logger, IOptions<CacheConfig> cacheConfig)
        {
            _connectionMultiplexer = connectionMultiplexer;
            _database = _connectionMultiplexer.GetDatabase();
            _logger = logger;

            var config = cacheConfig.Value;
            if (config.CacheExpirationSeconds <= 0)
                throw new InvalidOperationException("Invalid cache expiration time configuration.");

            _defaultExpirationTime = TimeSpan.FromSeconds(config.CacheExpirationSeconds);
        }

        public async Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default)
        {
            await SetAsync(key, value, _defaultExpirationTime, cancellationToken);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan expirationTime, CancellationToken cancellationToken = default)
        {
            var json = JsonSerializer.Serialize(value);
            await _database.StringSetAsync(key, json, expirationTime);

            _logger.LogInformationInterpolated($"Cache set for key: {key} with expiration: {expirationTime.TotalSeconds} seconds");
        }

        public async Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            var cachedData = await _database.StringGetAsync(key);
            if (cachedData.IsNullOrEmpty)
                return default;

            _logger.LogInformationInterpolated($"Cache hit for key: {key}");

            return JsonSerializer.Deserialize<T>(cachedData);
        }

        public async Task<T> GetOrSetAsync<T>(string key, Func<CancellationToken, Task<T>> getDataAsyncFunc, CancellationToken cancellationToken = default)
        {
            return await GetOrSetAsync(key, getDataAsyncFunc, _defaultExpirationTime, cancellationToken);
        }

        public async Task<T> GetOrSetAsync<T>(string key, Func<CancellationToken, Task<T>> getDataAsyncFunc, TimeSpan expirationTime, CancellationToken cancellationToken = default)
        {
            var cachedData = await GetAsync<T>(key, cancellationToken);
            if (cachedData != null)
                return cachedData;

            var fetchedData = await getDataAsyncFunc(cancellationToken);
            if (fetchedData != null)
                await SetAsync(key, fetchedData, expirationTime, cancellationToken);

            return fetchedData;
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            await _database.KeyDeleteAsync(key);

            _logger.LogInformationInterpolated($"Cache removed for key: {key}");
        }

        public async Task<bool> AcquireLockAsync(string lockKey, string lockValue, TimeSpan lockTtl, CancellationToken cancellationToken = default)
        {
            var acquired = await _database.StringSetAsync(lockKey, lockValue, lockTtl, when: When.NotExists);

            if (acquired)
                _logger.LogInformationInterpolated($"Lock acquired for key: {lockKey} with TTL: {lockTtl.TotalSeconds} seconds");
            else
                _logger.LogInformationInterpolated($"Failed to acquire lock for key: {lockKey} (already exists)");

            return acquired;
        }

        public async Task ReleaseLockAsync(string lockKey, CancellationToken cancellationToken = default)
        {
            await RemoveAsync(lockKey, cancellationToken);
            _logger.LogInformationInterpolated($"Lock released for key: {lockKey}");
        }

        public async Task<bool> RefreshKeyTtlAsync(string key, TimeSpan ttl, CancellationToken cancellationToken = default)
        {
            _logger.LogInformationInterpolated($"Update TTL for key {key}. New TTL: {ttl.TotalSeconds} seconds");
            return await _database.KeyExpireAsync(key, ttl);
        }
    }
}
