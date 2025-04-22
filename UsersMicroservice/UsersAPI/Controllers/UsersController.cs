using BusinessLogic.Contracts;
using BusinessLogic.Services.Interfaces;
using DataAccess.Common;
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
        private const int PageSize = 10;

        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize]
        [HttpPut("{userId}/profile")]
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

        [AuthorizeRoles(UserRoles.Admin)]
        [HttpGet]
        public async Task<IActionResult> GetAllPaged([FromQuery] int pageNumber = 1, CancellationToken token = default)
        {
            var pageParameters = new PageParameters { PageNumber = pageNumber, PageSize = PageSize };

            var users = await _userService.GetPagedAsync(pageParameters, token);

            return Ok(users);
        }
    }
}
