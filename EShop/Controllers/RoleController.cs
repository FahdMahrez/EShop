using EShop.Dto;
using EShop.Dto.RoleModel;
using EShop.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EShop.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Authorize(Roles = "Admin")]

    public class RoleController : ControllerBase
    {
        private readonly IRoleService roleService;
        private readonly ILogger<RoleController> logger;

        public RoleController(IRoleService roleService, ILogger<RoleController> logger)
        {
            this.roleService = roleService;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            var response = await roleService.GetAllAsync();
            if (!response.Success)
                return NotFound(response);

            return Ok(response);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetRoleById(Guid id)
        {
            var response = await roleService.GetByIdAsync(id);
            if (!response.Success)
                return NotFound(response);

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto createRoleDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(BaseResponse<RoleDto>.FailResponse("Invalid role data."));

            var response = await roleService.CreateAsync(createRoleDto);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateRole(Guid id, [FromBody] CreateRoleDto roleDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(BaseResponse<RoleDto>.FailResponse("Invalid role data."));

            var response = await roleService.UpdateAsync(id, roleDto);
            if (!response.Success)
                return NotFound(response);

            return Ok(response);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteRole(Guid id)
        {
            var response = await roleService.DeleteAsync(id);
            if (!response.Success)
                return NotFound(response);

            return Ok(response);
        }
    }
}
