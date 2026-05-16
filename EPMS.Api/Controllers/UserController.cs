using EPMS.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EPMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.GetAllUsersAsync();

            if (users == null || users.Count == 0)
            {
                return NotFound("No users found in the database.");
            }

            return Ok(users);
        }

    }
}
