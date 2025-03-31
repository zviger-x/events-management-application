using Application.Repositories.Interfaces;
using Domain.Entities;
using Infrastructure.Contexts;

namespace Infrastructure.Repositories
{
    public class SeatRepository : BaseRepository<Seat>, ISeatRepository
    {
        public SeatRepository(EventDbContext context)
            : base(context)
        {
        }
    }
}
