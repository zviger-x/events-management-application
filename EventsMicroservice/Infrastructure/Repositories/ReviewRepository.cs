using Application.Repositories.Interfaces;
using Domain.Entities;
using Infrastructure.Contexts;

namespace Infrastructure.Repositories
{
    public class ReviewRepository : BaseRepository<Review>, IReviewRepository
    {
        public ReviewRepository(EventDbContext context)
            : base(context)
        {
        }

        public async Task<PagedCollection<Review>> GetPagedByEventAsync(Guid eventId, int pageNumber, int pageSize, CancellationToken token = default)
        {
            return await GetPagedByFilterAsync(r => r.EventId == eventId, pageNumber, pageSize, token);
        }
    }
}
