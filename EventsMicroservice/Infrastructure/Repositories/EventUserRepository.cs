using Application.Repositories.Interfaces;
using Domain.Entities;
using Infrastructure.Contexts;
using Shared.Common;

namespace Infrastructure.Repositories
{
    public class EventUserRepository : BaseRepository<EventUser>, IEventUserRepository
    {
        public EventUserRepository(EventDbContext context)
            : base(context)
        {
        }

        public async Task<PagedCollection<EventUser>> GetPagedByEventAsync(Guid eventId, int pageNumber, int pageSize, CancellationToken token = default)
        {
            return await GetPagedByFilterAsync(e => e.EventId == eventId, pageNumber, pageSize, token);
        }
    }
}
