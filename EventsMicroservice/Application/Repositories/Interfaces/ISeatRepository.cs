using Domain.Entities;

namespace Application.Repositories.Interfaces
{
    public interface ISeatRepository : IRepository<Seat>
    {
        public Task CreateManyAsync(IEnumerable<Seat> seats, CancellationToken cancellationToken = default);
    }
}
