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
        [Consumes("multipart/form-data")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Create([FromForm] CreateProductDto request)
        {
            var response = await _productService.CreateAsync(request);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var response = await _productService.GetByIdAsync(id);
            return Ok(response);
        }

        [HttpGet("products")]
        public async Task<IActionResult> GetAllProducts()
        {
            var response = await _productService.GetAllAsync();
            return Ok(response);
        }

        [HttpGet("category/{categoryId:guid}")]
        public async Task<IActionResult> GetProductsByCategoryIdAsync([FromRoute]Guid categoryId)
        {
            var response = await _productService.GetProductsByCategoryIdAsync(categoryId);
            return Ok(response);
        }

        [HttpPut("update-product/{product-id}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Update([FromRoute(Name = "product-id")] Guid id, [FromBody] CreateProductDto request)
        {
            var response = await _productService.UpdateAsync(id, request);
            return Ok(response);
        }

        [HttpDelete("delete/{product-id}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var response = await _productService.DeleteAsync(id);
            return Ok(response);
        }
    }
}
