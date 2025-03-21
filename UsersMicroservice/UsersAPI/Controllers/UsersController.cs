using BusinessLogic.Caching.Constants;
using BusinessLogic.Caching.Interfaces;
using BusinessLogic.Contracts;
using BusinessLogic.Services.Interfaces;
using DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Security.Claims;
using System.Text.Json;
using UsersAPI.Filters;

namespace UsersAPI.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : Controller
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<UsersController> _logger;
        private readonly IUserService _userService;

        public UsersController(ICacheService cacheService, ILogger<UsersController> logger, IUserService userService)
        {
            _cacheService = cacheService;
            _logger = logger;
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
            await _cacheService.RemoveAsync(CacheKeys.UserById(id));

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
            var cachedUser = await _cacheService.GetAsync<User>(CacheKeys.UserById(id));
            if (cachedUser != null)
                return Ok(cachedUser);

            var user = await _userService.GetByIdAsync(id, token);
            if (user == null)
                return NotFound();

            await _cacheService.SetAsync(CacheKeys.UserById(id), user);

            return Ok(user);
        }

        [Authorize(Roles = nameof(UserRoles.Admin))]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var cachedUsers = await _cacheService.GetAsync<List<User>>(CacheKeys.AllUsers);

            if (cachedUsers != null)
            {
                _logger.LogInformation("------------ USERS FROM CACHE");
                return Ok(cachedUsers);
            }

            _logger.LogInformation("------------ USERS FROM DATABASE");
            var users = await _userService.GetAllAsync();
            await _cacheService.SetAsync(CacheKeys.AllUsers, users);

            return Ok(users);
        }

        [Authorize(Roles = nameof(UserRoles.Admin))]
        [HttpGet("{pageNumber}/{pageSize}")]
        public async Task<IActionResult> GetAllPaged(int pageNumber, int pageSize)
        {
            var cachedUsers = await _cacheService.GetAsync<PagedCollection<User>>(CacheKeys.PagedUsers(pageNumber, pageSize));

            if (cachedUsers != null)
                return Ok(cachedUsers);

            var users = await _userService.GetPagedAsync(pageNumber, pageSize);
            await _cacheService.SetAsync(CacheKeys.PagedUsers(pageNumber, pageSize), users);

            return Ok(users);
        }
    }
}
