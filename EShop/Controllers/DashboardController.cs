using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        [Authorize(Roles = "Admin")]
        [HttpGet("admin-dashboard")]
        public IActionResult GetAdminData()
        {
            return Ok("This data is only for admins.");
        }

        [Authorize(Roles = "User")]
        [HttpGet("user-profile")]
        public IActionResult GetUserData()
        {
            return Ok("This data is only for users.");
        }

        [Authorize(Roles = "Admin,User")]
        [HttpGet("shared")]
        public IActionResult GetSharedData()
        {
            return Ok("Both Admins and Users can access this.");
        }
    }
}
