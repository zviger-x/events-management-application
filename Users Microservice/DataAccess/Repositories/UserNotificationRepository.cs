using DataAccess.Contexts;
using DataAccess.Entities;

namespace DataAccess.Repositories
{
    internal class UserNotificationRepository : BaseRepository<UserNotification>
    {
        public UserNotificationRepository(UserDbContext context)
            : base(context)
        {
        }
        public override async Task<UserNotification> GetByIdAsync(int id)
        {
            return await _context.UserNotifications.FindAsync(id);
        }

        public override IQueryable<UserNotification> GetAll()
        {
            return _context.UserNotifications;
        }
    }
}
