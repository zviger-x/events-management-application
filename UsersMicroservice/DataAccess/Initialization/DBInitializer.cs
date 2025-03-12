using DataAccess.Contexts;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Initialization
{
    public class DBInitializer
    {
        private readonly UserDbContext _context;
        private readonly SqlServerConfig _config;

        public DBInitializer(UserDbContext context, SqlServerConfig config)
        {
            _context = context;
            _config = config;
        }

        public void Initialize()
        {
            // Если нужно пересоздать БД для демонстрационной части
            if (_config.RecreateDatabase)
            {
                _context.Database.EnsureDeleted();
                _context.Database.EnsureCreated();
            }

            // Заполнение демонстрационными данными (используется с пересозданием БД)
            if (_config.SeedDemoData && _config.RecreateDatabase)
                SeedDemoDataAsync();
        }

        private void SeedDemoDataAsync()
        {
            // В будущем заполнить пароли хеш версиями
            var demoUsers = new User[]
            {
                new User { Name = "Admin", Surname = "Admin", Email = "admin@gmail.com", Password = "invalid", Role = "admin" },
                new User { Name = "User1", Surname = "User1", Email = "user1@gmail.com", Password = "invalid", Role = "user" },
                new User { Name = "User2", Surname = "User2", Email = "user2@gmail.com", Password = "invalid", Role = "user" },
            };

            _context.AddRange(demoUsers);
            _context.SaveChanges();
        }
    }
}
