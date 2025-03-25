using Domain.Entities;
using MongoDB.Driver;

namespace Application.Contexts
{
    public class EventDbContext : BaseDbContext
    {
        public EventDbContext(IMongoDatabase database)
            : base(database)
        {
        }

        public IMongoCollection<Event> Events => Collection<Event>();
        public IMongoCollection<Seat> Seats => Collection<Seat>();
        public IMongoCollection<Review> Reviews => Collection<Review>();
    }
}
