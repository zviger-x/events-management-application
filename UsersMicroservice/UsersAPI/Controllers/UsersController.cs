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
            var response = await _userService.CreateAsync(user, token);
            if (response.HasErrors)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(User user, CancellationToken token)
        {
            var response = await _userService.UpdateAsync(user, token);
            if (response.HasErrors)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken token)
        {
            var response = await _userService.DeleteAsync(id, token);
            if (response.HasErrors)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken token)
        {
            var response = await _userService.GetByIdAsync(id, token);
            if (response.HasErrors)
                return NotFound(response);

            if (response.DataTransferObject == null)
                return NotFound();

            return Ok(response);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var response = _userService.GetAll();
            if (response.HasErrors)
                return BadRequest(response);

            return Ok(response);
        }
    }
}
