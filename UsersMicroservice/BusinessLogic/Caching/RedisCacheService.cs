using BusinessLogic.Caching.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BusinessLogic.Caching
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<RedisCacheService> _logger;

        private readonly TimeSpan _defaultExpirationTime;

        public RedisCacheService(IConfiguration configuration, IDistributedCache cache, ILogger<RedisCacheService> logger)
        {
            _cache = cache;
            _logger = logger;

            var cacheExpirationSeconds = configuration.GetValue<int?>("Caching:CacheExpirationSeconds");
            if (cacheExpirationSeconds == null)
                throw new InvalidOperationException($"The {nameof(cacheExpirationSeconds)} key is missing or its value is invalid.");

            _defaultExpirationTime = TimeSpan.FromSeconds(cacheExpirationSeconds.Value);
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var cachedData = await _cache.GetStringAsync(key);
            if (string.IsNullOrEmpty(cachedData)) return default;

            _logger.LogInformation($"Cache hit for key: {key}");
            return JsonSerializer.Deserialize<T>(cachedData);
        }

        public async Task SetAsync<T>(string key, T value)
        {
            var options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(_defaultExpirationTime);
            var serializedData = JsonSerializer.Serialize(value);
            await _cache.SetStringAsync(key, serializedData, options);

            _logger.LogInformation($"Cache set for key: {key} with expiration: {_defaultExpirationTime.TotalSeconds} seconds");
        }

        public async Task RemoveAsync(string key)
        {
            await _cache.RemoveAsync(key);
            _logger.LogInformation($"Cache removed for key: {key}");
        }
    }
}
