using EShop.Dto.CategoryModel;
using EShop.Repositories.Interface;
using EShop.Services;
using EShop.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EShop.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        public readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpPost("create")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Create([FromBody] CreateCategoryDto request)
        {
            var response = await _categoryService.CreateAsync(request);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var response = await _categoryService.GetByIdAsync(id);
            return Ok(response);
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetAll()
        {
            var response = await _categoryService.GetAllAsync();
            return Ok(response);
        }

        [HttpPut("{Id:guid}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Update(Guid id, [FromBody] CreateCategoryDto request)
        {
            var response = await _categoryService.UpdateAsync(id, request);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _categoryService.DeleteAsync(id);
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}
