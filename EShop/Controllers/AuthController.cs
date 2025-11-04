using EShop.Dto;
using EShop.Dto.UserModel;
using EShop.Repositories;
using EShop.Repositories.Interface;
using EShop.Services;
using EShop.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;

namespace EShop.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IUserRoleService _userRoleService;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;

        public AuthController(IUserService userService, IUserRoleService userRoleService, ITokenService tokenService, IConfiguration configuration)
        {
            _userService = userService;
            _userRoleService = userRoleService;
            _tokenService = tokenService;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDto userDto, CancellationToken cancellationToken)
        {
            Log.Information("Attempting to register user with email: {Email}", userDto.Email);

            var result = await _userService.CreateAsync(userDto);
            if (!result.Success || result.Data == null)
            {
                Log.Warning("User registration failed for email: {Email}. Reason: {Message}", userDto.Email, result.Message);
                return BadRequest(result.Message);
            }
            var user = result.Data;

            var rolesResponse = await _userRoleService.GetRolesByUserIdAsync(user.Id, CancellationToken.None);
            var userRoles = rolesResponse?.Data ?? new List<string>();

            if (!userRoles.Contains("User"))
            {
                var roleRepo = HttpContext.RequestServices.GetRequiredService<IRoleRepository>();
                var userRoleRepo = HttpContext.RequestServices.GetRequiredService<IUserRoleRepository>();

                var roles = await roleRepo.GetAllAsync(CancellationToken.None);
                var userRole = roles.FirstOrDefault(r => r.RoleName == "User");

                if (userRole != null)
                    await userRoleRepo.AssignRoleToUserAsync(user.Id, userRole.Id, CancellationToken.None);
            }

            Log.Information("User registered successfully with default role: User");
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto,CancellationToken cancellationToken)
        {
            Log.Information("Login attempt for email: {Email}", loginDto.Email);

            var userResponse = await _userService.ValidateUserAsync(loginDto.Email, loginDto.Password, cancellationToken);

            if (!userResponse.Success || userResponse.Data == null)
            {
                Log.Warning("Login failed for email: {Email} - Invalid credentials", loginDto.Email);
                return Unauthorized("Invalid email or password");
            }
            var user = userResponse.Data;

            var rolesResponse = await _userRoleService.GetRolesByUserIdAsync(user.Id, cancellationToken);
            var roles = rolesResponse.Data ?? new List<string>();

            var token = _tokenService.GenerateToken(user, roles);
            var refreshToken = _tokenService.GenerateRefreshToken();
            var expiry = DateTime.UtcNow.AddMinutes(60);

            await _userService.UpdateRefreshTokenAsync(user.Id, refreshToken, expiry, cancellationToken);

            return Ok(new
            {
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = expiry,
                Roles = roles,
                Email = user.Email
            });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            Log.Information("Refresh token request received.");
            
            if (string.IsNullOrEmpty(request.RefreshToken))
                return BadRequest("Refresh token is required.");

            var userResponse = await _userService.GetByRefreshTokenAsync(request.RefreshToken, CancellationToken.None);
           
            if (!userResponse.Success || userResponse.Data == null)
            {
                Log.Warning("Invalid refresh token attempt.");
                return Unauthorized("Invalid refresh token.");
            }

            var user = userResponse.Data;

            if (user.RefreshTokenExpiry < DateTime.UtcNow)
            {
                Log.Warning("Expired refresh token for user {Email}", user.Email);
                return Unauthorized("Expired refresh token.");
            }

            var rolesResponse = await _userRoleService.GetRolesByUserIdAsync(user.Id, CancellationToken.None);
            var roles = rolesResponse.Data ?? new List<string>();

            var newToken = _tokenService.GenerateToken(user, roles);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            var newExpiry = DateTime.UtcNow.AddDays(7);

            var updateResponse = await _userService.UpdateRefreshTokenAsync(user.Id, newRefreshToken, newExpiry, CancellationToken.None);
            if (!updateResponse.Success)
            {
                Log.Error("Failed to refresh token for user {Email}", user.Email);
                return StatusCode(500, "Could not refresh token.");
            }

            Log.Information("Token refreshed successfully for user {Email}", user.Email);

            return Ok(new
            {
                Token = newToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = newExpiry,
                Message = "Token refreshed successfully."
            });
        }
    }
}
