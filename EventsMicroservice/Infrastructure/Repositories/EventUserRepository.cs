using Application.Repositories.Interfaces;
using Domain.Entities;
using Infrastructure.Contexts;
using MongoDB.Driver;
using Shared.Common;

namespace Infrastructure.Repositories
{
    public class EventUserRepository : BaseRepository<EventUser>, IEventUserRepository
    {
        public EventUserRepository(EventDbContext context)
            : base(context)
        {
        }

        public async Task<EventUser> GetByUserAndEventAsync(Guid userId, Guid eventId, CancellationToken cancellationToken = default)
        {
            var filter = Builders<EventUser>.Filter.Where(eu => eu.UserId == userId && eu.EventId == eventId);
            using var cursor = await _context.EventUsers.FindAsync(filter, cancellationToken: cancellationToken);
            return await cursor.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<PagedCollection<EventUser>> GetPagedByEventAsync(Guid eventId, int pageNumber, int pageSize, CancellationToken token = default)
        {
            return await GetPagedByFilterAsync(e => e.EventId == eventId, pageNumber, pageSize, token);
        }
    }
}
