using DataAccess.Contexts;
using DataAccess.Entities;
using DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class UserNotificationRepository : BaseRepository<UserNotification>, IUserNotificationRepository
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
            return _context.UserNotifications.AsNoTracking();
        }
    }
}
