using BusinessLogic.Caching.Interfaces;
using BusinessLogic.Contracts;
using BusinessLogic.Services.Interfaces;
using DataAccess.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UsersAPI.Attributes;

namespace UsersAPI.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;
        private readonly IUserNotificationService _userNotificationService;
        private readonly IUserTransactionService _userTransactionService;

        private const int PageSize = 10;

        public UsersController(ICacheService cacheService,
            IUserService userService,
            IUserNotificationService userNotificationService,
            IUserTransactionService userTransactionService)
        {
            _userService = userService;
            _userNotificationService = userNotificationService;
            _userTransactionService = userTransactionService;
        }

        [Authorize]
        [HttpPatch("{userId}/profile")]
        public async Task<IActionResult> ChangeProfile([FromRoute] Guid userId, [FromBody] UpdateUserDTO updateUserDTO, CancellationToken token)
        {
            await _userService.UpdateUserProfileAsync(userId, updateUserDTO, token);

            return Ok();
        }

        [Authorize]
        [HttpPatch("{userId}/password")]
        public async Task<IActionResult> ChangePassword([FromRoute] Guid userId, [FromBody] ChangePasswordDTO changePasswordDTO, CancellationToken token)
        {
            await _userService.ChangePasswordAsync(userId, changePasswordDTO, token);

            return Ok();
        }

        [AuthorizeRoles(UserRoles.Admin)]
        [HttpPatch("{userId}/role")]
        public async Task<IActionResult> ChangeRole([FromRoute] Guid userId, [FromBody] ChangeUserRoleDTO changeUserRoleDTO, CancellationToken token)
        {
            await _userService.ChangeUserRoleAsync(userId, changeUserRoleDTO, token);

            return Ok();
        }

        [Authorize]
        [HttpDelete("{userId}")]
        public async Task<IActionResult> Delete([FromRoute] Guid userId, CancellationToken token)
        {
            await _userService.DeleteAsync(userId, token);

            return NoContent();
        }

        [Authorize]
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetById([FromRoute] Guid userId, CancellationToken token)
        {
            var user = await _userService.GetByIdAsync(userId, token);

            return Ok(user);
        }

        [Authorize]
        [HttpGet("{userId}/notifications")]
        public async Task<IActionResult> GetUserNotifications([FromRoute] Guid userId, CancellationToken token)
        {
            var notifications = await _userNotificationService.GetByUserIdAsync(userId, token);

            return Ok(notifications);
        }

        [Authorize]
        [HttpGet("{userId}/transactions")]
        public async Task<IActionResult> GetUserTransactions([FromRoute] Guid userId, CancellationToken token)
        {
            var transactions = await _userTransactionService.GetByUserIdAsync(userId, token);

            return Ok(transactions);
        }

        [AuthorizeRoles(UserRoles.Admin)]
        [HttpGet]
        public async Task<IActionResult> GetAllPaged([FromQuery] int pageNumber = 1, CancellationToken token = default)
        {
            var users = await _userService.GetPagedAsync(pageNumber, PageSize, token);

            return Ok(users);
        }
    }
}
