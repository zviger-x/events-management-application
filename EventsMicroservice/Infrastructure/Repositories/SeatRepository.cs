using Application.Repositories.Interfaces;
using Domain.Entities;
using Infrastructure.Contexts;
using Infrastructure.Extensions;

namespace Infrastructure.Repositories
{
    public class SeatRepository : BaseRepository<Seat>, ISeatRepository
    {
        public SeatRepository(EventDbContext context, TransactionContext transactionContext)
            : base(context, transactionContext)
        {
        }
    }
}
