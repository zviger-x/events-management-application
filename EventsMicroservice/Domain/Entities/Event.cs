using Shared.Entities.Interfaces;

namespace Domain.Entities
{
    public class Event : IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public string Location { get; set; }
        public DateTimeOffset PurchaseDeadline { get; set; }
        public byte[] Image { get; set; }
        public IEnumerable<Seat> Seats { get; set; }
        public IEnumerable<EventComment> Comments { get; set; }
    }
}
