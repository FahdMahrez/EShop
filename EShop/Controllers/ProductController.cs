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
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost("create-product")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Create([FromBody] CreateProductDto request)
        {
            if (request == null)
                return BadRequest("Product data cannot be null.");

            var response = await _productService.CreateAsync(request);

            if (!response.Success) 
                return BadRequest(response.Message);

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var response = await _productService.GetByIdAsync(id);

            if (!response.Success)
                return NotFound(response.Message);

            return Ok(response);
        }

        [HttpGet("products")]
        public async Task<IActionResult> GetAllProducts()
        {
            var response = await _productService.GetAllAsync();
            if (!response.Success)
                return NotFound(new { message = response.Message });

            return Ok(response);
       
        }

        [HttpGet("category/{categoryId:guid}")]
        public async Task<IActionResult> GetProductsByCategoryIdAsync([FromRoute]Guid categoryId)
        {
            if (categoryId == Guid.Empty)
                return BadRequest("Invalid category ID.");

            var response = await _productService.GetProductsByCategoryIdAsync(categoryId);

            if (!response.Success)
                return NotFound(new { message = response.Message });

            return Ok(response);
        }

        [HttpPut("update-product/{product-id}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Update([FromRoute(Name = "product-id")] Guid id, [FromBody] CreateProductDto request)
        {
            if (request == null)
                return BadRequest("Invalid product data.");

            var response = await _productService.UpdateAsync(id, request);

            if (!response.Success)
                return BadRequest(response.Message);

            return Ok(response);
        }

        [HttpDelete("delete/{product-id}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var response = await _productService.DeleteAsync(id);

            if (!response.Success)
                return NotFound(response.Message);

            return Ok(response);
        }
    }
}
