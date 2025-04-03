using Application.Repositories.Interfaces;
using Domain.Entities;
using Infrastructure.Contexts;

namespace Infrastructure.Repositories
{
    public class EventCommentRepository : BaseRepository<EventComment>, IEventCommentRepository
    {
        public EventCommentRepository(EventDbContext context, TransactionContext transactionContext)
            : base(context, transactionContext)
        {
        }

        public async Task<PagedCollection<EventComment>> GetPagedByEventAsync(Guid eventId, int pageNumber, int pageSize, CancellationToken token = default)
        {
            return await GetPagedByFilterAsync(r => r.EventId == eventId, pageNumber, pageSize, token);
        }
    }
}
