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
                _context.Database.Migrate();
            }
            
            // Заполнение демонстрационными данными (используется с пересозданием БД)
            if (_config.SeedDemoData && _config.RecreateDatabase)
                SeedDemoDataAsync();
        }

        private void SeedDemoDataAsync()
        {
            // В будущем заполнить пароли хеш версиями
            var users = new User[]
            {
                new User { Name = "Admin", Surname = "Admin", Email = "admin@gmail.com", PasswordHash = "invalid", Role = UserRoles.Admin },
                new User { Name = "User1", Surname = "User1", Email = "user1@gmail.com", PasswordHash = "invalid", Role = UserRoles.User },
                new User { Name = "User2", Surname = "User2", Email = "user2@gmail.com", PasswordHash = "invalid", Role = UserRoles.User },
            };
            _context.AddRange(users);

            var notifications = new UserNotification[]
            {
                new UserNotification { UserId = users[0].Id, Message = "Notification_User_0_0", DateTime = DateTime.Now, Status = "read"},
                new UserNotification { UserId = users[0].Id, Message = "Notification_User_0_1", DateTime = DateTime.Now, Status = "notread"},
                new UserNotification { UserId = users[1].Id, Message = "Notification_User_1_0", DateTime = DateTime.Now, Status = "notread"},
                new UserNotification { UserId = users[1].Id, Message = "Notification_User_1_1", DateTime = DateTime.Now, Status = "notread"},
                new UserNotification { UserId = users[1].Id, Message = "Notification_User_1_2", DateTime = DateTime.Now, Status = "read"},
                new UserNotification { UserId = users[2].Id, Message = "Notification_User_2_0", DateTime = DateTime.Now, Status = "read"},
            };
            _context.AddRange(notifications);

            var transactions = new UserTransaction[]
            {
                new UserTransaction { UserId = users[0].Id, EventId = Guid.NewGuid(), SeatNumber = 0, SeatRow = 0, Amount = 1.99f, TransactionDate = DateTime.Now },
                new UserTransaction { UserId = users[1].Id, EventId = Guid.NewGuid(), SeatNumber = 0, SeatRow = 0, Amount = 1.99f, TransactionDate = DateTime.Now },
                new UserTransaction { UserId = users[1].Id, EventId = Guid.NewGuid(), SeatNumber = 0, SeatRow = 0, Amount = 5.99f, TransactionDate = DateTime.Now },
                new UserTransaction { UserId = users[2].Id, EventId = Guid.NewGuid(), SeatNumber = 0, SeatRow = 0, Amount = 19.99f, TransactionDate = DateTime.Now },
            };
            _context.AddRange(transactions);

            _context.SaveChanges();
        }
    }
}
