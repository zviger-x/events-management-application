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
        private readonly IDistributedCache _cache;
        private readonly ILogger<UsersController> _logger;
        private readonly IUserService _userService;

        public UsersController(IDistributedCache cache, ILogger<UsersController> logger, IUserService userService)
        {
            _cache = cache;
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
            var cachedUsers = await _cache.GetStringAsync("users_cache_key");

            if (!string.IsNullOrEmpty(cachedUsers))
            {
                _logger.LogInformation("------------ USERS FROM CACHE");
                var users = JsonSerializer.Deserialize<List<User>>(cachedUsers);
                return Ok(users);
            }

            _logger.LogInformation("------------ USERS FROM DATABASE");
            var usersFromDb = await _userService.GetAllAsync();

            var cacheExpiryOptions = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(10));
            await _cache.SetStringAsync("users_cache_key", JsonSerializer.Serialize(usersFromDb), cacheExpiryOptions);

            return Ok(usersFromDb);
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
