using EShop.Dto;
using EShop.Dto.UserModel;
using EShop.Services;
using EShop.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace EShop.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IUserRoleService _userRoleService;
        private readonly ITokenService _tokenService;


        public UserController(IUserService userService, IUserRoleService userRoleService, ITokenService tokenService)
        {
            _userService = userService;
            _userRoleService = userRoleService;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (string.IsNullOrWhiteSpace(loginDto.Email) || string.IsNullOrWhiteSpace(loginDto.Password))
                return BadRequest(BaseResponse<string>.FailResponse("Email and password are required."));

            Log.Information("User {Email} attempting to log in", loginDto.Email);

            var userResponse = await _userService.ValidateUserAsync(loginDto.Email, loginDto.Password, CancellationToken.None);
            if (!userResponse.Success || userResponse.Data == null)
                return Unauthorized(BaseResponse<string>.FailResponse("Invalid email or password."));

            var user = userResponse.Data;

            var rolesResponse = await _userRoleService.GetRolesByUserIdAsync(user.Id, CancellationToken.None);
            if (!rolesResponse.Success)
                return BadRequest(BaseResponse<string>.FailResponse("Failed to retrieve user roles."));

            var roles = rolesResponse.Data ?? Enumerable.Empty<string>();
            var token = _tokenService.GenerateToken(user, roles);
            var refreshToken = _tokenService.GenerateRefreshToken();

            await _userService.UpdateRefreshTokenAsync(user.Id, refreshToken, DateTime.UtcNow.AddDays(7), CancellationToken.None);

            Log.Information("User {Email} logged in successfully", loginDto.Email);

            return Ok(BaseResponse<object>.SuccessResponse(new
            {
                Token = token,
                RefreshToken = refreshToken,
                Message = "Login successful"
            }, "User logged in successfully."));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> GetAllUsers()
        {
            var response = await _userService.GetAllAsync();
            return Ok(response);
        }

        [HttpGet("{id:guid}")]

        public async Task<IActionResult> GetUserById(Guid id)
        {
            var response = await _userService.GetByIdAsync(id, CancellationToken.None);
            return Ok(response);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateUser([FromBody] UserDto userDto)
        {
            var response = await _userService.CreateAsync(userDto);
            return Ok(response);
        }

        [HttpPost("{userId:guid}/assign-role/{roleId:guid}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> AssignRoleToUser(Guid userId, Guid roleId)
        {
            var result = await _userRoleService.AssignRoleToUserAsync(userId, roleId, CancellationToken.None);
            return Ok(result);
        }

        [HttpGet("{userId:guid}/roles")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> GetRolesForUser(Guid userId)
        {
            var rolesResponse = await _userRoleService.GetRolesByUserIdAsync(userId, CancellationToken.None);
            return Ok(rolesResponse);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UserDto userDto)
        {
            var response = await _userService.UpdateAsync(id, userDto);
            return Ok(response);
        }

        [HttpGet("products")]
        public async Task<IActionResult> GetAllProducts()
        {
            var result = await _userService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            var result = await _userService.GetAllAsync();
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var response = await _userService.DeleteAsync(id);
            return Ok(response);
        }
    }
}
