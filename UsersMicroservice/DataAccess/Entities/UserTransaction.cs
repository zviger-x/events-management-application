using DataAccess.Entities.Interfaces;

#pragma warning disable CS8618
namespace DataAccess.Entities
{
    public class UserTransaction : IEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid EventId { get; set; }
        public int SeatRow { get; set; }
        public int SeatNumber { get; set; }
        public float Amount { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
