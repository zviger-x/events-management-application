using DataAccess.Entities.Interfaces;

namespace DataAccess.Entities
{
    // TODO: EventName должен обновлятся через Kafka, при непосредственном обновлении данных ивента
    public class UserTransaction : IEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid EventId { get; set; }
        public string EventName { get; set; } = default!;
        public int SeatRow { get; set; }
        public int SeatNumber { get; set; }
        public float Amount { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
