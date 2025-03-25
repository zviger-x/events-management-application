using Domain.Entities.Interfaces;

#pragma warning disable CS8618
namespace Domain.Entities
{
    public class Seat : IEntity
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public int Row { get; set; }
        public int Number { get; set; }
        public float Price { get; set; }
        public string Status { get; set; }
    }
}
