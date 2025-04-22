using BusinessLogic.Services.Interfaces;
using DataAccess.Common;
using DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UsersAPI.Controllers
{
    // TODO: Создать gRPC сервис для работы с уведомлениями
    // Этот класс для демонстрации и проверки корректности работы CRUD операций с уведомлениями.
    // Он будет перенесён в gRPC сервис, потому что пользователю нет смысла взаимодействовать с этими методами.
    [ApiController]
    [Route("api/notifications")]
    public class NotificationsController : Controller
    {
        private const int PageSize = 10;

        private readonly IUserNotificationService _userNotificationService;

        public NotificationsController(IUserNotificationService userNotificationService)
        {
            _userNotificationService = userNotificationService;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserNotification notification, CancellationToken cancellationToken)
        {
            await _userNotificationService.CreateAsync(notification, cancellationToken);

            return Ok();
        }

        [Authorize]
        [HttpPut("{notificationId}")]
        public async Task<IActionResult> Update([FromRoute] Guid notificationId, [FromBody] UserNotification notification, CancellationToken cancellationToken)
        {
            await _userNotificationService.UpdateAsync(notificationId, notification, cancellationToken);

            return Ok();
        }

        [Authorize]
        [HttpDelete("{notificationId}")]
        public async Task<IActionResult> Delete([FromRoute] Guid notificationId, CancellationToken cancellationToken)
        {
            await _userNotificationService.DeleteAsync(notificationId, cancellationToken);

            return NoContent();
        }

        [Authorize]
        [HttpGet("{notificationId}")]
        public async Task<IActionResult> GetById([FromRoute] Guid notificationId, CancellationToken cancellationToken)
        {
            var notification = await _userNotificationService.GetByIdAsync(notificationId, cancellationToken);

            return Ok(notification);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, CancellationToken cancellationToken = default)
        {
            var pageParameters = new PageParameters { PageNumber = pageNumber, PageSize = PageSize };

            var notifications = await _userNotificationService.GetPagedAsync(pageParameters, cancellationToken);

            return Ok(notifications);
        }

        [Authorize]
        [HttpGet("/api/users/{userId}/notifications")]
        public async Task<IActionResult> GetUserNotifications([FromRoute] Guid userId, CancellationToken token)
        {
            var notifications = await _userNotificationService.GetByUserIdAsync(userId, token);

            return Ok(notifications);
        }

    }
}
