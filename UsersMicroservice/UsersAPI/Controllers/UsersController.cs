using BusinessLogic.Caching.Interfaces;
using BusinessLogic.Contracts;
using BusinessLogic.Services.Interfaces;
using DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UsersAPI.Attributes;
using UsersAPI.Filters;

namespace UsersAPI.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;

        private const int PageSize = 10;

        public UsersController(ICacheService cacheService, IUserService userService)
        {
            _userService = userService;
        }

        [Authorize]
        [ForCurrentUserOrRoles(UserRoles.Admin)]
        [HttpPut("{id}/profile")]
        public async Task<IActionResult> ChangeProfile([FromRoute] Guid id, [FromBody] UpdateUserDTO updateUserDTO, CancellationToken token)
        {
            if (id != updateUserDTO.Id)
                throw new ArgumentException("You are not allowed to modify this profile.");

            await _userService.UpdateUserProfileAsync(updateUserDTO, token);

            return Ok();
        }

        [Authorize]
        [ForCurrentUserOrRoles(UserRoles.Admin)]
        [HttpPut("{id}/password")]
        public async Task<IActionResult> ChangePassword([FromRoute] Guid id, [FromBody] ChangePasswordDTO changePasswordDTO, CancellationToken token)
        {
            if (id != changePasswordDTO.Id)
                throw new ArgumentException("You are not allowed to modify this profile.");

            await _userService.ChangePasswordAsync(changePasswordDTO, token);

            return Ok();
        }

        [Authorize]
        [ForCurrentUserOrRoles(UserRoles.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken token)
        {
            await _userService.DeleteAsync(id, token);

            return Ok();
        }

        [Authorize]
        [ForCurrentUserOrRoles(UserRoles.Admin)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken token)
        {
            var user = await _userService.GetByIdAsync(id, token);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [AuthorizeRoles(UserRoles.Admin)]
        [HttpGet("all")]
        public async Task<IActionResult> GetAll(CancellationToken token)
        {
            var users = await _userService.GetAllAsync(token);

            return Ok(users);
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
