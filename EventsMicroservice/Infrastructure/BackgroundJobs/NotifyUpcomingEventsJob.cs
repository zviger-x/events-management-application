using Application.MediatR.Queries.EventQueries;
using Application.MediatR.Queries.EventUserQueries;
using Application.Messages;
using Domain.Entities;
using Infrastructure.BackgroundJobs.Interfaces;
using MediatR;
using Shared.Kafka.Contracts.Events;

namespace Infrastructure.BackgroundJobs
{
    public class NotifyUpcomingEventsJob : INotifyUpcomingEventsJob
    {
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
            var windowStart = now.AddHours(-23);
            var windowEnd = now;

            var events = await _mediator.Send(command, cancellationToken);
            var eventsInInterval = events.Where(e => e.EndDate >= windowStart && e.EndDate <= windowEnd);

            foreach (var evt in eventsInInterval)
            {
                await NotifyUsersAsync(evt, cancellationToken);
            }
        }

        private async Task NotifyUsersAsync(Event evt, CancellationToken cancellationToken)
        {
            var command = new EventUserGetAllByEventQuery { EventId = evt.Id };

            var users = (await _mediator.Send(command, cancellationToken)).Select(eu => eu.UserId);

            var dto = new EventUpcomingDto
            {
                StartTime = evt.StartDate,
                EventId = evt.Id,
                Name = evt.Name,
                TargetUsers = users
            };

            await _eventUpcomingMessageProducer.PublishAsync(dto, cancellationToken);
        }
    }
}
