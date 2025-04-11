using Application.Repositories.Interfaces;
using Domain.Entities;
using Infrastructure.Contexts;

namespace Infrastructure.Repositories
{
    public class SeatConfigurationRepository : BaseRepository<SeatConfiguration>, ISeatConfigurationRepository
    {
        public SeatConfigurationRepository(EventDbContext context)
            : base(context)
        {
        }
    }
}
