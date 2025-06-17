using Domain.Entities;
using Infrastructure.Mongo.Interfaces;
using MongoDB.Driver;

namespace Infrastructure.Contexts
{
    public class EventDbContext : BaseDbContext
    {
        public EventDbContext(IMongoDatabase database, IServiceProvider serviceProvider)
            : base(database, serviceProvider)
        {
        }

        public IMongoCollectionWrapper<Event> Events => Collection<Event>();
        public IMongoCollectionWrapper<EventComment> EventComments => Collection<EventComment>();
        public IMongoCollectionWrapper<EventUser> EventUsers => Collection<EventUser>();
        public IMongoCollectionWrapper<Seat> Seats => Collection<Seat>();
    }
}
