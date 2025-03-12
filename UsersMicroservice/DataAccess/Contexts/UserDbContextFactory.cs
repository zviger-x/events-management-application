using DataAccess.Initialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DataAccess.Contexts
{
    public class UserDbContextFactory : IDesignTimeDbContextFactory<UserDbContext>
    {
        public UserDbContext CreateDbContext(string[] args)
        {
            // dotnet ef migrations add [name] --project DataAccess --startup-project UsersAPI
            // dotnet ef migrations remove --project DataAccess --startup-project UsersAPI

            // Тянем строку подключения из проекта UsersAPI
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "UsersAPI");
            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var sqlConfig = configuration.GetSection("SqlServerConfig").Get<SqlServerConfig>();
            var connectionString = sqlConfig?.ConnectionString;
            var optionsBuilder = new DbContextOptionsBuilder<UserDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new UserDbContext(optionsBuilder.Options);
        }
    }
}
