using EShop.Dto.CategoryModel;
using EShop.Dto.ProductModel;
using EShop.Services;
using EShop.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EShop.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Authorize(Roles = "Admin")]

    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IUserRoleService _userRoleService;

        public AdminController(IUserService userService, IProductService productService, ICategoryService categoryService, IUserRoleService userRoleService)
        {
            _userService = userService;
            _productService = productService;
            _categoryService = categoryService;
            _userRoleService = userRoleService;
        }

        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole(Guid userId, Guid roleId, CancellationToken cancellationToken)
        {
            var result = await _userRoleService.AssignRoleToUserAsync(userId, roleId, cancellationToken);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _userService.GetAllAsync();
            return Ok(result);
        }

        [HttpPost("products")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddProduct([FromForm] CreateProductDto request)
        {
            var result = await _productService.CreateAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("products/{id:guid}")]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] CreateProductDto request)
        {
            var result = await _productService.UpdateAsync(id, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("products/{id:guid}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var result = await _productService.DeleteAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("categories")]
        public async Task<IActionResult> AddCategory([FromBody] CreateCategoryDto request)
        {
            var result = await _categoryService.CreateAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
