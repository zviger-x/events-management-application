using Application.Repositories.Interfaces;
using Domain.Entities;
using MongoDB.Driver;
using Infrastructure.Contexts;
using Infrastructure.Extensions;

#pragma warning disable CS8603
namespace Infrastructure.Repositories
{
    public class EventRepository : BaseRepository<Event>, IEventRepository
    {
        public EventRepository(EventDbContext context)
            : base(context)
        {
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
                    _context.GetCollectionName<Seat>(),
                    e => e.Id,
                    r => r.EventId,
                    e => e.Reviews);

            return await query.FirstOrDefaultAsync();
        }
    }
}
