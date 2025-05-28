using BusinessLogic.Contracts;
using BusinessLogic.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Common;
using Shared.Extensions;

namespace UsersAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/notifications")]
    public class NotificationsController : Controller
    {
        private const int PageSize = 10;

        private readonly IUserNotificationService _userNotificationService;

        public NotificationsController(IUserNotificationService userNotificationService)
        {
            _userNotificationService = userNotificationService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserNotificationDto notification, CancellationToken cancellationToken)
        {
            var createdId = await _userNotificationService.CreateAsync(notification, cancellationToken);

            return StatusCode(StatusCodes.Status201Created, new { Id = createdId });
        }

        [HttpPut("{notificationId}")]
        public async Task<IActionResult> Update([FromRoute] Guid notificationId, [FromBody] UpdateUserNotificationDto notification, CancellationToken cancellationToken)
        {
            await _userNotificationService.UpdateAsync(notificationId, notification, cancellationToken);

            return Ok();
        }

        [HttpDelete("{notificationId}")]
        public async Task<IActionResult> Delete([FromRoute] Guid notificationId, CancellationToken cancellationToken)
        {
            await _userNotificationService.DeleteAsync(notificationId, cancellationToken);

            return NoContent();
        }

        [HttpGet("{notificationId}")]
        public async Task<IActionResult> GetById([FromRoute] Guid notificationId, CancellationToken cancellationToken)
        {
            var notification = await _userNotificationService.GetByIdAsync(notificationId, cancellationToken);

            return Ok(notification);
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, CancellationToken cancellationToken = default)
        {
            var pageParameters = new PageParameters { PageNumber = pageNumber, PageSize = PageSize };

            var notifications = await _userNotificationService.GetPagedAsync(pageParameters, cancellationToken);

            return Ok(notifications);
        }

        [HttpGet("/api/users/{userId}/notifications")]
        public async Task<IActionResult> GetUserNotifications([FromRoute] Guid userId, CancellationToken token)
        {
            var currentUserId = User.GetUserIdOrThrow();
            var isAdmin = User.IsAdminOrThrow();

            var notifications = await _userNotificationService.GetByUserIdAsync(userId, currentUserId, isAdmin, token);

            return Ok(notifications);
        }

    }
}
