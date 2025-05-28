namespace Application.Contracts
{
    public class ProcessPaymentDto
    {
        public required string Token { get; set; }
        public required Guid UserId { get; set; }
        public required Guid EventId { get; set; }
        public required string EventName { get; set; }
        public required int SeatRow { get; set; }
        public required int SeatNumber { get; set; }
        public required float Amount { get; set; }
    }
}
