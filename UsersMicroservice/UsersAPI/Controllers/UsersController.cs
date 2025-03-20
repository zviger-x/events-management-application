using BusinessLogic.Contracts;
using BusinessLogic.Services.Interfaces;
using DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UsersAPI.Filters;

namespace UsersAPI.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize]
        [ForCurrentUserOrRoles(UserRoles.Admin)]
        [HttpPut("{id}/change-profile")]
        public async Task<IActionResult> ChangeProfile(Guid id, UpdateUserDTO updateUserDTO, CancellationToken token)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) || userId != id)
                return Forbid();

            if (id != updateUserDTO.Id)
                throw new ArgumentException("ID in URL does not match ID in model");

            await _userService.UpdateUserProfileAsync(updateUserDTO, token);

            return Ok();
        }

        [Authorize]
        [ForCurrentUserOrRoles(UserRoles.Admin)]
        [HttpPut("{id}/change-password")]
        public async Task<IActionResult> ChangePassword(Guid id, ChangePasswordDTO changePasswordDTO, CancellationToken token)
        {
            if (id != changePasswordDTO.Id)
                throw new ArgumentException("ID in URL does not match ID in model");

            await _userService.ChangePasswordAsync(changePasswordDTO, token);

            return Ok();
        }

        [Authorize]
        [ForCurrentUserOrRoles(UserRoles.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken token)
        {
            await _userService.DeleteAsync(id, token);

            return Ok();
        }

        [Authorize]
        [ForCurrentUserOrRoles(UserRoles.Admin)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken token)
        {
            var user = await _userService.GetByIdAsync(id, token);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [Authorize(Roles = nameof(UserRoles.Admin))]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();

            return Ok(users);
        }

        [Authorize(Roles = nameof(UserRoles.Admin))]
        [HttpGet("{pageNumber}/{pageSize}")]
        public async Task<IActionResult> GetAllPaged(int pageNumber, int pageSize)
        {
            var users = await _userService.GetPagedAsync(pageNumber, pageSize);

            return Ok(users);
        }
    }
}
