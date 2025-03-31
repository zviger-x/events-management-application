using Domain.Entities.Interfaces;

namespace Domain.Entities
{
    public class Seat : IEntity
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public int Row { get; set; }
        public int Number { get; set; }
        public float Price { get; set; }
        public bool IsBought { get; set; }
    }
}
