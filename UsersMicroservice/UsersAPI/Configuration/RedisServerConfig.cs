namespace UsersAPI.Configuration
{
    internal class RedisServerConfig
    {
        public string ConnectionString { get; set; } = default!;
        public string CachePrefix { get; set; } = default!;
    }
}
