using Application.Repositories.Interfaces;
using Domain.Entities;
using Infrastructure.Contexts;

#pragma warning disable CS8603
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
