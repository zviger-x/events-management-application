using DataAccess.Contexts;
using DataAccess.Entities;
using DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(UserDbContext context)
            : base(context)
        {
        }

        public override Task DeleteAsync(Guid id)
        {
            var user = new User() { Id = id };
            _context.Users.Remove(user);
            return Task.CompletedTask;
        }

        public override async Task<User> GetByIdAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        public override IQueryable<User> GetAll()
        {
            return _context.Users.AsNoTracking();
        }
    }
}
