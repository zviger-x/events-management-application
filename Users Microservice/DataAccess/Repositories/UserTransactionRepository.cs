using DataAccess.Contexts;
using DataAccess.Entities;

namespace DataAccess.Repositories
{
    internal class UserTransactionRepository : BaseRepository<UserTransaction>
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
            return _context.UserTransactions;
        }
    }
}
