namespace DataAccess.Initialization
{
    #nullable disable
    public class SqlServerConfig
    {
        public string ConnectionString { get; set; }
        public bool RecreateDatabase { get; set; }
        public bool SeedDemoData { get; set; }
    }
}
