using Application.MediatR.Queries.EventQueries;
using Application.MediatR.Queries.EventUserQueries;
using Domain.Entities;
using Infrastructure.BackgroundJobs.Interfaces;
using MediatR;
using Shared.Extensions;

namespace Infrastructure.BackgroundJobs.Common
{
    public abstract class BaseEventNotifierJob<TKafkaDto> : IBackgroundJob
    {
        protected const int MaxDegreeOfParallelismForEvents = 20;
        protected const int MaxDegreeOfParallelismForUsers = 100;
        protected const int UsersBatchSize = 1000;

        protected readonly IMediator _mediator;

        protected BaseEventNotifierJob(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var events = await _mediator.Send(new EventGetAllQuery(), cancellationToken);

            var window = GetWindow();
            var eventsInInterval = events.Where(e => IsEventInWindow(e.EndDate, window.Start, window.End));

            var options = new ParallelOptions
            {
                MaxDegreeOfParallelism = MaxDegreeOfParallelismForEvents,
                CancellationToken = cancellationToken
            };

            await Parallel.ForEachAsync(eventsInInterval, options, async (@event, ct) =>
            {
                await NotifyUsersAsync(@event, ct);
            });
        }

        private async Task NotifyUsersAsync(Event @event, CancellationToken cancellationToken)
        {
            var command = new EventUserGetAllByEventQuery { EventId = @event.Id };

            var users = (await _mediator.Send(command, cancellationToken))
                .Select(eu => eu.UserId)
                .Batch(UsersBatchSize);

            var options = new ParallelOptions
            {
                MaxDegreeOfParallelism = MaxDegreeOfParallelismForUsers,
                CancellationToken = cancellationToken
            };

            await Parallel.ForEachAsync(users, options, async (batch, ct) =>
            {
                var dto = CreateDto(@event, batch);

                await PublishAsync(dto, ct);
            });
        }

        protected abstract (DateTime Start, DateTime End) GetWindow();

        protected abstract TKafkaDto CreateDto(Event @event, IEnumerable<Guid> users);

        protected abstract Task PublishAsync(TKafkaDto dto, CancellationToken cancellationToken);

        private static bool IsEventInWindow(DateTimeOffset currentDate, DateTime windowStart, DateTime windowEnd)
        {
            var now = currentDate.ToUniversalTime();

            return now >= windowStart && now <= windowEnd;
        }
    }
}
