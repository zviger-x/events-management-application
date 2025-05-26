using DataAccess.Contexts;
using DataAccess.Entities;
using DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class UserTransactionRepository : BaseRepository<UserTransaction>, IUserTransactionRepository
    {
        public UserTransactionRepository(UserDbContext context)
            : base(context)
        {
        }

        public async Task<IEnumerable<UserTransaction>> GetByUserIdAsync(Guid id, CancellationToken token = default)
        {
            return await _context.UserTransactions
                .AsNoTracking()
                .Where(t => t.UserId == id)
                .ToListAsync(token);
        }
    }
}
