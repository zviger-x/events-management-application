using Application.Contracts;
using MediatR;

namespace Application.MediatR.Commands
{
    public class ProcessPaymentCommand : IRequest<PaymentResultDto>
    {
        public Guid UserId { get; set; }
        public Guid EventId { get; set; }
        public string EventName { get; set; }
        public string Token { get; set; }
        public float Amount { get; set; }
        public int SeatRow { get; set; }
        public int SeatNumber { get; set; }
    }
}
