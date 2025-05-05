using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Caching.Locks;
using Shared.Caching.Locks.Interfaces;
using Shared.Caching.Services.Interfaces;
using Shared.Configuration;
using Shared.Logging.Extensions;
using StackExchange.Redis;
using System.Text.Json;

namespace Shared.Caching.Services
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

        public async Task SetAsync<T>(string key, T value, bool ignoreDefaultExpirationTime, CancellationToken cancellationToken = default)
        {
            var expiry = ignoreDefaultExpirationTime ? (TimeSpan?)_defaultExpirationTime : null;

            await SetAsyncWithTtl(key, value, expiry, cancellationToken);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan expirationTime, CancellationToken cancellationToken = default)
        {
            await SetAsyncWithTtl(key, value, expirationTime, cancellationToken);
        }

        public async Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

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
            cancellationToken.ThrowIfCancellationRequested();

            await _database.KeyDeleteAsync(key);

            _logger.LogInformationInterpolated($"Cache removed for key: {key}");
        }

        public async Task<IRedisLock> AcquireLockAsync(string lockKey, string lockValue, TimeSpan lockTtl, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var isAcquired = await _database.LockTakeAsync(lockKey, lockValue, lockTtl);
            if (!isAcquired)
            {
                _logger.LogInformationInterpolated($"Failed to acquire lock for key: {lockKey} (already exists)");
                return null;
            }

            _logger.LogInformationInterpolated($"Lock acquired for key: {lockKey} with TTL: {lockTtl.TotalSeconds} seconds");
            return new RedisLock(_database, lockKey, lockValue, lockTtl);
        }

        public async Task AddToSetAsync(string setKey, string value, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await _database.SetAddAsync(setKey, value);
        }

        public async Task RemoveFromSetAsync(string setKey, string value, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await _database.SetRemoveAsync(setKey, value);
        }

        public async Task<IEnumerable<T>> GetSetMembersAsync<T>(string setKey, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var members = await _database.SetMembersAsync(setKey);

            var results = new List<T>(members.Length);
            foreach (var key in members)
            {
                var item = await GetAsync<T>(key, cancellationToken);

                if (item != null)
                    results.Add(item);
            }

            return results;
        }

        private async Task SetAsyncWithTtl<T>(string key, T value, TimeSpan? expirationTime, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var json = JsonSerializer.Serialize(value);

            if (expirationTime.HasValue)
            {
                await _database.StringSetAsync(key, json, expirationTime);

                _logger.LogInformationInterpolated($"Cache set for key: {key} with expiration: {expirationTime.Value.TotalSeconds} seconds");
            }
            else
            {
                await _database.StringSetAsync(key, json);

                _logger.LogInformationInterpolated($"Cache set for key: {key} with infinite lifetime");
            }
        }
    }
}
