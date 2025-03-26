using Application.Repositories.Interfaces;
using Domain.Entities;
using Infrastructure.Contexts;

#pragma warning disable CS8603
namespace Infrastructure.Repositories
{
    public class ReviewRepository : BaseRepository<Review>, IReviewRepository
    {
        public ReviewRepository(EventDbContext context) : base(context)
        {
        }
    }
}
