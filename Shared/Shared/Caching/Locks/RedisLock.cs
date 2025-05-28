using Shared.Caching.Locks.Interfaces;
using StackExchange.Redis;
using static Shared.Logging.Extensions.SerilogExtensions;

namespace Shared.Caching.Locks
{
    internal class RedisLock : IRedisLock
    {
        private const double RefreshFactor = 0.7;

        private readonly IDatabase _db;
        private readonly string _key;
        private readonly string _token;
        private readonly TimeSpan _expiry;
        private readonly CancellationTokenSource _cts;
        private readonly Task _extensionTask;

        public string Key => _key;

        /// <summary>
        /// Creates a Redis lock instance with the provided database, key, token, and expiry time.
        /// </summary>
        /// <param name="db">The Redis database instance.</param>
        /// <param name="key">The key used for the lock.</param>
        /// <param name="token">The unique token for the lock.</param>
        /// <param name="expiry">The expiration time for the lock.</param>
        public RedisLock(IDatabase db, string key, string token, TimeSpan expiry)
        {
            _db = db;
            _key = key;
            _token = token;
            _expiry = expiry;
            _cts = new();
            _extensionTask = Task.Run(ExtendLockAsync);
        }

        /// <summary>
        /// Continuously extends the lock until it is canceled.
        /// </summary>
        private async Task ExtendLockAsync()
        {
            Log.InformationInterpolated($"The task to extend the life of TTL for key {_key} has been launched.");

            var refreshDelay = TimeSpan.FromTicks((long)(_expiry.Ticks * RefreshFactor));

            while (!_cts.Token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(refreshDelay, _cts.Token);
                    await _db.LockExtendAsync(_key, _token, _expiry);
                    Log.InformationInterpolated($"The TTL for key {_key} has been extended.");
                }
                catch (TaskCanceledException)
                {
                    Log.InformationInterpolated($"The task to extend the life of TTL for key {_key} has been completed.");
                    break;
                }
                catch (Exception ex)
                {
                    Log.WarningInterpolated($"Failed to extend Redis lock key \"{_key}\": {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Releases the Redis lock and cancels the extension task.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            _cts.Cancel();
            await _extensionTask;
            await _db.LockReleaseAsync(_key, _token);

            Log.InformationInterpolated($"Lock released for key: {_key}");
        }
    }
}
