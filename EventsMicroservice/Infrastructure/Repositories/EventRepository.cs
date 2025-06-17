using Application.Repositories.Interfaces;
using Application.Specifications.Interfaces;
using Domain.Entities;
using Infrastructure.Contexts;
using Infrastructure.Extensions;
using MongoDB.Driver;
using Shared.Common;

namespace Infrastructure.Repositories
{
    public class EventRepository : BaseRepository<Event>, IEventRepository
    {
        public EventRepository(EventDbContext context)
            : base(context)
        {
        }

        public override async Task DeleteAsync(Event entity, CancellationToken token = default)
        {
            using var session = await _context.Client.StartSessionAsync(cancellationToken: token);

            session.StartTransaction();
            try
            {
                var deleteEventResult = await _context.Events.DeleteOneAsync(session, e => e.Id == entity.Id, cancellationToken: token);

                if (deleteEventResult.DeletedCount > 0)
                {
                    await _context.Seats.DeleteManyAsync(session, s => s.EventId == entity.Id, cancellationToken: token);
                    await _context.EventComments.DeleteManyAsync(session, ec => ec.EventId == entity.Id, cancellationToken: token);
                    await _context.EventUsers.DeleteManyAsync(session, eu => eu.EventId == entity.Id, cancellationToken: token);
                }

                await session.CommitTransactionAsync(token);
            }
            catch
            {
                await session.AbortTransactionAsync(token);
                throw;
            }
        }

        public override async Task DeleteManyAsync(IEnumerable<Event> entity, CancellationToken token = default)
        {
            using var session = await _context.Client.StartSessionAsync(cancellationToken: token);

            session.StartTransaction();
            try
            {
                var ids = entity.Select(e => e.Id);
                var successfullyDeletedIds = new List<Guid>();

                foreach (var id in ids)
                {
                    var result = await _context.Events.DeleteOneAsync(session, e => e.Id == id, cancellationToken: token);
                    if (result.DeletedCount > 0)
                        successfullyDeletedIds.Add(id);
                }

                // Удаляем связанные данные только для успешно удалённых event'ов
                if (successfullyDeletedIds.Any())
                {
                    await _context.Seats.DeleteManyAsync(session, s => successfullyDeletedIds.Contains(s.EventId), cancellationToken: token);
                    await _context.EventComments.DeleteManyAsync(session, ec => successfullyDeletedIds.Contains(ec.EventId), cancellationToken: token);
                }

                await session.CommitTransactionAsync(token);
            }
            catch
            {
                await session.AbortTransactionAsync(token);
                throw;
            }
        }

        public override async Task<Event> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            var query = _context.Events
                .Aggregate()
                .Match(e => e.Id == id)
                .Lookup<Event, Seat, Guid, Guid, IEnumerable<Seat>>(
                    _context.GetCollectionName<Seat>(),
                    e => e.Id,
                    s => s.EventId,
                    e => e.Seats)
                .Lookup<Event, EventComment, Guid, Guid, IEnumerable<EventComment>>(
                    _context.GetCollectionName<EventComment>(),
                    e => e.Id,
                    ec => ec.EventId,
                    e => e.Comments);

            return await query.FirstOrDefaultAsync(token);
        }

        public async Task<PagedCollection<Event>> GetPagedByFilterAsync(ISpecification<Event> specification, int pageNumber, int pageSize, CancellationToken token = default)
        {
            var expression = specification.ToExpression();

            return await GetPagedByFilterAsync(expression, pageNumber, pageSize, token);
        }
    }
}
