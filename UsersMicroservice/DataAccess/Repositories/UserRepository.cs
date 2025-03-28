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

        public override async Task<User> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            return (await _context.Users
                .AsNoTracking()
                .Include(u => u.Notifications)
                .Include(u => u.Transactions)
                .FirstOrDefaultAsync(u => u.Id == id, token))!;
        }

        public async Task<bool> ContainsEmailAsync(string email, CancellationToken token = default)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<User> GetByEmailAsync(string email, CancellationToken token = default)
        {
            return (await _context.Users.SingleOrDefaultAsync(u => u.Email == email))!;
        }
    }
}
