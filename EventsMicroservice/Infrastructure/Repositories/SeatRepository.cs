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

        public async Task CreateManyAsync(IEnumerable<Seat> seats, CancellationToken cancellationToken = default)
        {
            await _context.Seats.InsertManyWithSessionAsync(_transactionContext.CurrentSession, seats, cancellationToken: cancellationToken);
        }
    }
}
