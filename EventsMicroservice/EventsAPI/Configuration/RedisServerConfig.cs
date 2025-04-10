namespace EventsAPI.Configuration
{
    public class RedisServerConfig
    {
        public string ConnectionString { get; set; } = default!;
        public string CachePrefix { get; set; } = default!;
    }
}
