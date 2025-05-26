namespace BusinessLogic.Contracts
{
    public class UpdateUserTransactionDto
    {
        public int SeatRow { get; set; }
        public int SeatNumber { get; set; }
        public float Amount { get; set; }
    }
}
