using Application.Contracts;
using MediatR;

namespace Application.MediatR.Commands
{
    public class SendNotificationCommand : IRequest
    {
        public required NotificationDto Notification { get; set; }

#warning TODO: Убрать выброс ошибки!
        public required bool ThrowError { get; set; }
    }
}
