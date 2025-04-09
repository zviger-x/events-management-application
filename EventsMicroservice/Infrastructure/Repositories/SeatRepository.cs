using Application.Repositories.Interfaces;
using Domain.Entities;
using Infrastructure.Contexts;
using Infrastructure.Extensions;
using MongoDB.Driver;

namespace Infrastructure.Repositories
{
    public class SeatRepository : BaseRepository<Seat>, ISeatRepository
    {
        public SeatRepository(EventDbContext context, TransactionContext transactionContext)
            : base(context, transactionContext)
        {
        }

        public async Task<IEnumerable<Seat>> GetByEventAsync(Guid eventId, CancellationToken cancellationToken = default)
        {
            var filter = Builders<Seat>.Filter.Eq(s => s.EventId, eventId);
            using var cursor = await _context.Seats.FindWithSessionAsync(CurrentSession, filter, cancellationToken: cancellationToken);
            return await cursor.ToListAsync(cancellationToken);
        }
    }
}
