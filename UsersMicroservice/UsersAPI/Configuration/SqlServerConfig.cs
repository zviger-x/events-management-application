namespace UsersAPI.Configuration
{
    public class SqlServerConfig
    {
        public string ConnectionString { get; set; }
        public bool RecreateDatabase { get; set; }
        public bool SeedDemoData { get; set; }
    }
}
