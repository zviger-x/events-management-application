using BusinessLogic.Services.Interfaces;
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
        private readonly IUserNotificationService _userNotificationService;

        private const int PageSize = 10;

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
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UserNotification notification, CancellationToken cancellationToken)
        {
            if (id != notification.Id)
                throw new ArgumentException("You are not allowed to modify this transaction.");

            await _userNotificationService.UpdateAsync(notification, cancellationToken);

            return Ok();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            await _userNotificationService.DeleteAsync(id, cancellationToken);

            return Ok();
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var notification = await _userNotificationService.GetByIdAsync(id, cancellationToken);
            if (notification == null)
                return NotFound();

            return Ok(notification);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, CancellationToken cancellationToken = default)
        {
            var notifications = await _userNotificationService.GetPagedAsync(pageNumber, PageSize, cancellationToken);

            return Ok(notifications);
        }
    }
}
