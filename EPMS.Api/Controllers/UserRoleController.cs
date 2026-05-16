using EPMS.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EPMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRoleController : ControllerBase
    {
        private readonly IUserRoleService _userRoleService;

        public UserRoleController(IUserRoleService userRoleService)
        {
            _userRoleService = userRoleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserRoles()
        {
            var userRoles = await _userRoleService.GetAllUserRolesAsync();

            if (userRoles == null || userRoles.Count == 0)
            {
                return NotFound("No user roles found.");
            }

            return Ok(userRoles);
        }
    }
}
