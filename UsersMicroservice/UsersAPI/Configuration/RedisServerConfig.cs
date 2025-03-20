namespace UsersAPI.Configuration
{
    #pragma warning disable CS8618
    internal class RedisServerConfig
    {
        public string ConnectionString { get; set; }
        public string CachePrefix { get; set; }
    }
}
