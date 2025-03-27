using Domain.Entities.Interfaces;

#pragma warning disable CS8618
namespace Domain.Entities
{
    public class Event : IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; }
        public DateTime PurchaseDeadline { get; set; }
        public byte[]? Image { get; set; }
        public IEnumerable<Seat> Seats { get; set; }
        public IEnumerable<Review> Reviews { get; set; }
    }
}
