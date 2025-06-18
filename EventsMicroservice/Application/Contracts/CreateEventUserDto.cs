namespace Application.Contracts
{
    public class CreateEventUserDto
    {
        public Guid EventId { get; set; }
        public Guid UserId { get; set; }
        public Guid SeatId { get; set; }
    }
}
