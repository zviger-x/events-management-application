using Application.MediatR.Queries.EventQueries;
using Application.MediatR.Queries.EventUserQueries;
using Application.Messages;
using Domain.Entities;
using Infrastructure.BackgroundJobs.Interfaces;
using MediatR;
using Shared.Kafka.Contracts.Events;

namespace Infrastructure.BackgroundJobs
{
    public class NotifyCompletedEventsJob : INotifyCompletedEventsJob
    {
        private readonly IMediator _mediator;
        private readonly IEventCompletedMessageProducer _eventCompletedMessageProducer;

        public NotifyCompletedEventsJob(IMediator mediator, IEventCompletedMessageProducer eventCompletedMessageProducer)
        {
            _mediator = mediator;
            _eventCompletedMessageProducer = eventCompletedMessageProducer;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var command = new EventGetAllQuery();

            var now = DateTime.UtcNow;
            var windowStart = now.AddHours(-1) + TimeSpan.FromTicks(1);
            var windowEnd = now;

            var events = await _mediator.Send(command, cancellationToken);
            var eventsInInterval = events.Where(e =>
                e.EndDate.ToUniversalTime() >= windowStart &&
                e.EndDate.ToUniversalTime() <= windowEnd);

            foreach (var evt in eventsInInterval)
            {
                await NotifyUsersAsync(evt, cancellationToken);
            }
        }

        private async Task NotifyUsersAsync(Event evt, CancellationToken cancellationToken)
        {
            var command = new EventUserGetAllByEventQuery { EventId = evt.Id };

            var users = (await _mediator.Send(command, cancellationToken)).Select(eu => eu.UserId);

            var dto = new EventCompletedDto
            {
                CompletedAt = evt.EndDate,
                EventId = evt.Id,
                Name = evt.Name,
                TargetUsers = users
            };

            await _eventCompletedMessageProducer.PublishAsync(dto, cancellationToken);
        }
    }
}
