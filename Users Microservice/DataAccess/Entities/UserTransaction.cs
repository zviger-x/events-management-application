using DataAccess.Entities.Interfaces;

namespace DataAccess.Entities
{
    public class UserTransaction : IEntity
    {
        public int Id { get; set; }
        public required int UsertId { get; set; }
        public required int EventId { get; set; }
        public required int SeatRow { get; set; }
        public required int SeatNumber { get; set; }
        public required float Amount { get; set; }
        public required DateTime TransactionDate { get; set; }
    }
}
