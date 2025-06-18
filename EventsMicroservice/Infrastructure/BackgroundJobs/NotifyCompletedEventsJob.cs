using Application.Messages;
using Domain.Entities;
using Infrastructure.BackgroundJobs.Common;
using Infrastructure.BackgroundJobs.Interfaces;
using MediatR;
using Shared.Kafka.Contracts.Events;

namespace Infrastructure.BackgroundJobs
{
    public class NotifyCompletedEventsJob : BaseEventNotifierJob<EventCompletedDto>, INotifyCompletedEventsJob
    {
        private readonly IEventCompletedMessageProducer _producer;

        public NotifyCompletedEventsJob(IMediator mediator, IEventCompletedMessageProducer producer)
            : base(mediator)
        {
            _producer = producer;
        }

        protected override EventCompletedDto CreateDto(Event @event, IEnumerable<Guid> users)
        {
            return new EventCompletedDto
            {
                CompletedAt = @event.StartDate,
                EventId = @event.Id,
                Name = @event.Name,
                TargetUsers = users
            };
        }

        protected override (DateTime Start, DateTime End) GetWindow()
        {
            var now = DateTime.UtcNow;
            var windowStart = now.AddHours(-1).AddTicks(1);

            return new(windowStart, now);
        }

        protected override async Task PublishAsync(EventCompletedDto dto, CancellationToken cancellationToken)
        {
            await _producer.PublishAsync(dto, cancellationToken);
        }
    }
}
