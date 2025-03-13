using DataAccess.Entities.Interfaces;

namespace DataAccess.Entities
{
    public class UserTransaction : IEntity
    {
        public int Id { get; set; }
        public int UsertId { get; set; }
        public int EventId { get; set; }
        public int SeatRow { get; set; }
        public int SeatNumber { get; set; }
        public float Amount { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
