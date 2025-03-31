using Application.Repositories.Interfaces;
using Domain.Entities;
using Infrastructure.Contexts;

namespace Infrastructure.Repositories
{
    public class ReviewRepository : BaseRepository<Review>, IReviewRepository
    {
        public ReviewRepository(EventDbContext context) : base(context)
        {
        }
    }
}
