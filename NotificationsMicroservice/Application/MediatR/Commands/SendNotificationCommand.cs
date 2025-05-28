using Application.Contracts;
using MediatR;

namespace Application.MediatR.Commands
{
    public class SendNotificationCommand : IRequest
    {
        public required NotificationDto Notification { get; set; }
    }
}
