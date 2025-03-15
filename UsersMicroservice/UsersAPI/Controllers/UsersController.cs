using BusinessLogic.Services.Interfaces;
using DataAccess.Entities;
using Microsoft.AspNetCore.Mvc;

namespace UsersAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, User user, CancellationToken token)
        {
            if (id != user.Id)
                throw new ArgumentException("ID in URL does not match ID in model");

            await _userService.UpdateAsync(user, token);

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
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();

            return Ok(users);
        }
    }
}
