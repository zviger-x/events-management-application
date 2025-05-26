using Application.MediatR.Queries.EventQueries;
using Application.MediatR.Queries.EventUserQueries;
using Application.Messages;
using Domain.Entities;
using Infrastructure.BackgroundJobs.Interfaces;
using MediatR;
using Shared.Extensions;
using Shared.Kafka.Contracts.Events;

namespace Infrastructure.BackgroundJobs
{
    public class NotifyUpcomingEventsJob : INotifyUpcomingEventsJob
    {
        private const int MaxDegreeOfParallelismForEvents = 20;
        private const int MaxDegreeOfParallelismForUsers = 100;
        private const int UsersBatchSize = 1000;

        private readonly IMediator _mediator;
        private readonly IEventUpcomingMessageProducer _eventUpcomingMessageProducer;

        public NotifyUpcomingEventsJob(IMediator mediator, IEventUpcomingMessageProducer eventUpcomingMessageProducer)
        {
            _mediator = mediator;
            _eventUpcomingMessageProducer = eventUpcomingMessageProducer;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var command = new EventGetAllQuery();

            var now = DateTime.UtcNow;
            var windowStart = now.AddDays(-1) + TimeSpan.FromTicks(1);
            var windowEnd = now;

            var events = await _mediator.Send(command, cancellationToken);
            var eventsInInterval = events.Where(e => IsEventInWindow(e.EndDate, windowStart, windowEnd));

            var options = new ParallelOptions
            {
                MaxDegreeOfParallelism = MaxDegreeOfParallelismForEvents,
                CancellationToken = cancellationToken
            };

            await Parallel.ForEachAsync(
                eventsInInterval,
                options,
                async (evt, ct) =>
                {
                    await NotifyUsersAsync(evt, ct);
                });
        }

        private async Task NotifyUsersAsync(Event evt, CancellationToken cancellationToken)
        {
            var command = new EventUserGetAllByEventQuery { EventId = evt.Id };

            var users = (await _mediator.Send(command, cancellationToken))
                .Select(eu => eu.UserId)
                .Batch(UsersBatchSize);

            var options = new ParallelOptions
            {
                MaxDegreeOfParallelism = MaxDegreeOfParallelismForUsers,
                CancellationToken = cancellationToken
            };

            await Parallel.ForEachAsync(
                users,
                options,
                async (currentBatch, ct) =>
                {
                    var dto = new EventUpcomingDto
                    {
                        StartTime = evt.StartDate,
                        EventId = evt.Id,
                        Name = evt.Name,
                        TargetUsers = currentBatch
                    };

                    await _eventUpcomingMessageProducer.PublishAsync(dto, ct);
                });
        }

        private static bool IsEventInWindow(DateTimeOffset currentDate, DateTime windowStart, DateTime windowEnd)
        {
            var now = currentDate.ToUniversalTime();

            return now >= windowStart && now <= windowEnd;
        }
    }
}
