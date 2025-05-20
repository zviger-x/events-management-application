namespace Application.Contracts
{
    public class CreateUserTransactionDto
    {
        public Guid UserId { get; set; }
        public Guid EventId { get; set; }
        public string EventName { get; set; }
        public int SeatRow { get; set; }
        public int SeatNumber { get; set; }
        public float Amount { get; set; }
        public DateTime TransactionDate => DateTime.UtcNow;
    }
}
