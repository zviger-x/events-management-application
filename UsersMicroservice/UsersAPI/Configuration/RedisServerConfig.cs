namespace UsersAPI.Configuration
{
    internal class RedisServerConfig
    {
        public string ConnectionString { get; set; }
        public string CachePrefix { get; set; }
    }
}
