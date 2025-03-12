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
        public override async Task<UserTransaction> GetByIdAsync(int id)
        {
            return await _context.UserTransactions.FindAsync(id);
        }

        public override IQueryable<UserTransaction> GetAll()
        {
            return _context.UserTransactions.AsNoTracking();
        }
    }
}
