using BusinessLogic.Contracts;
using BusinessLogic.Services.Interfaces;
using DataAccess.Entities;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost]
        public async Task<IActionResult> Create(User user, CancellationToken token)
        {
            await _userService.CreateAsync(user, token);

            return Ok();
        }

        [HttpPut("{id}/change-profile")]
        public async Task<IActionResult> ChangeProfile(Guid id, UpdateUserDTO updateUserDTO, CancellationToken token)
        {
            if (id != updateUserDTO.Id)
                throw new ArgumentException("ID in URL does not match ID in model");

            await _userService.UpdateUserProfileAsync(updateUserDTO, token);

            return Ok();
        }

        [HttpPut("{id}/change-password")]
        public async Task<IActionResult> ChangePassword(Guid id, ChangePasswordDTO changePasswordDTO, CancellationToken token)
        {
            if (id != changePasswordDTO.Id)
                throw new ArgumentException("ID in URL does not match ID in model");

            await _userService.ChangePasswordAsync(changePasswordDTO, token);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken token)
        {
            await _userService.DeleteAsync(id, token);

            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken token)
        {
            var user = await _userService.GetByIdAsync(id, token);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();

            return Ok(users);
        }

        [HttpGet("{pageNumber}/{pageSize}")]
        public async Task<IActionResult> GetAllPaged(int pageNumber, int pageSize)
        {
            var users = await _userService.GetPagedAsync(pageNumber, pageSize);

            return Ok(users);
        }
    }
}
