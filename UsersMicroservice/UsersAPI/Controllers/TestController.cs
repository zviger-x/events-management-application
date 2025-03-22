using DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UsersAPI.Attributes;

namespace UsersAPI.Controllers
{
    [ApiController]
    [Route("api/test")]
    public class TestController : Controller
    {
        [Authorize]
        [HttpGet("authorization")]
        public IActionResult TestDefault() => Ok();

        [AuthorizeRoles(UserRoles.Admin)]
        [HttpGet("authorization-admin")]
        public IActionResult TestAdmin() => Ok();

        [AuthorizeRoles(UserRoles.User, UserRoles.Admin)]
        [HttpGet("authorization-user&admin")]
        public IActionResult TestUser() => Ok();
    }
}
