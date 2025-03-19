using DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UsersAPI.Controllers
{
    [ApiController]
    [Route("api/test")]
    public class TestController : Controller
    {
        [Authorize]
        [HttpGet("authorization")]
        public IActionResult TestDefault() => Ok();

        [Authorize(Roles = nameof(UserRoles.Admin))]
        [HttpGet("authorization-admin")]
        public IActionResult TestAdmin() => Ok();

        [Authorize(Roles = $"{nameof(UserRoles.Admin)},{nameof(UserRoles.User)}")]
        [HttpGet("authorization-user")]
        public IActionResult TestUser() => Ok();
    }
}
