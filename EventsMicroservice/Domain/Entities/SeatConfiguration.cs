using Shared.Entities.Interfaces;

namespace Domain.Entities
{
    public class SeatConfiguration : IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public float DefaultPrice { get; set; }
        public List<int> Rows { get; set; } // Количество мест в каждом ряду
    }
}
