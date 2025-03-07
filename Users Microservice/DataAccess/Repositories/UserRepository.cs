using DataAccess.Contexts;
using DataAccess.Entities;

namespace DataAccess.Repositories
{
    internal class UserRepository : BaseRepository<User>
    {
        public UserRepository(UserDbContext context)
            : base(context)
        {
        }
        public override async Task<User> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public override IQueryable<User> GetAll()
        {
            return _context.Users;
        }
    }
}
