using Application.Messages;
using Domain.Entities;
using Infrastructure.BackgroundJobs.Common;
using Infrastructure.BackgroundJobs.Interfaces;
using MediatR;
using Shared.Kafka.Contracts.Events;

namespace Infrastructure.BackgroundJobs
{
    public class NotifyUpcomingEventsJob : BaseEventNotifierJob<EventUpcomingDto>, INotifyUpcomingEventsJob
    {
        private readonly IEventUpcomingMessageProducer _producer;

        public NotifyUpcomingEventsJob(IMediator mediator, IEventUpcomingMessageProducer producer)
            : base(mediator)
        {
            _producer = producer;
        }

        protected override EventUpcomingDto CreateDto(Event @event, IEnumerable<Guid> users)
        {
            return new EventUpcomingDto
            {
                StartTime = @event.StartDate,
                EventId = @event.Id,
                Name = @event.Name,
                TargetUsers = users
            };
        }

        protected override (DateTime Start, DateTime End) GetWindow()
        {
            var now = DateTime.UtcNow;
            var windowStart = now.AddDays(-1).AddTicks(1);

            return new(windowStart, now);
        }

        protected override async Task PublishAsync(EventUpcomingDto dto, CancellationToken cancellationToken)
        {
            await _producer.PublishAsync(dto, cancellationToken);
        }
    }
}
