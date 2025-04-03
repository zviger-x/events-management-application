using Application.Repositories.Interfaces;
using Domain.Entities;
using MongoDB.Driver;
using Infrastructure.Contexts;
using Infrastructure.Extensions;

namespace Infrastructure.Repositories
{
    public class EventRepository : BaseRepository<Event>, IEventRepository
    {
        public EventRepository(EventDbContext context)
            : base(context)
        {
        }

        public override async Task DeleteAsync(Event entity, CancellationToken token = default)
        {
            using var session = await _context.Client.StartSessionAsync(cancellationToken: token);

            session.StartTransaction();
            try
            {
                var deleteEventResult = await _context.Events.DeleteOneAsync(session, e => e.Id == entity.Id, cancellationToken: token);

                if (deleteEventResult.DeletedCount > 0)
                {
                    await _context.Seats.DeleteManyAsync(session, s => s.EventId == entity.Id, cancellationToken: token);
                    await _context.Reviews.DeleteManyAsync(session, r => r.EventId == entity.Id, cancellationToken: token);
                }

                await session.CommitTransactionAsync(token);
            }
            catch
            {
                await session.AbortTransactionAsync(token);
                throw;
            }
        }

        public override async Task<Event> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            var query = _context.Events
                .Aggregate()
                .Match(e => e.Id == id)
                .Lookup<Event, Seat, Guid, Guid, IEnumerable<Seat>>(
                    _context.GetCollectionName<Seat>(),
                    e => e.Id,
                    s => s.EventId,
                    e => e.Seats)
                .Lookup<Event, Review, Guid, Guid, IEnumerable<Review>>(
                    _context.GetCollectionName<Review>(),
                    e => e.Id,
                    r => r.EventId,
                    e => e.Reviews);

            return await query.FirstOrDefaultAsync();
        }
    }
}
