using Microsoft.AspNetCore.Mvc;
using EPMS.Application.Interfaces;


namespace EPMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        // Constructor Injection သုံးပြီး RoleService ကို လှမ်းခေါ်ခြင်း
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        // GET: api/role
        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _roleService.GetAllRolesAsync();

            if (roles == null || roles.Count == 0)
            {
                return NotFound("No roles found in the database.");
            }

            return Ok(roles);
        }
    }
}
