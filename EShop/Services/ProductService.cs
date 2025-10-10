using EShop.Data;
using EShop.Dto;
using EShop.Dto.ProductModel;
using EShop.Repositories;
using EShop.Repositories.Interface;
using EShop.Services.Interface;

namespace EShop.Services
{
    public class ProductService(IProductRepository productRepository, ILogger<ProductService> logger) : IProductService
    {
        public async Task<BaseResponse<bool>> CreateAsync(CreateProductDto request)
        {
            try
            {
                if ((request == null))
                {
                    return BaseResponse<bool>.FailResponse("Invalid product data.");

                }
                var product = new Product
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,    
                    Description = request.Description,
                    SellingPrice = request.SellingPrice,
                    CostPrice = request.CostPrice,
                    StockQuantity = request.StockQuantity,
                    CategoryId = request.CategoryId,
                    CreatedAt = DateTime.Now,
                };

                var result = await productRepository.AddAsync(product, CancellationToken.None);

                if ((!result))
                {
                    return BaseResponse<bool>.FailResponse("Failed to create product.");
                }
                return BaseResponse<bool>.SuccessResponse(true, "Product created successfully.");
            }
            catch (Exception ex)
            {
                logger.LogCritical($"Error: {ex.InnerException?.Message ?? ex.Message}");
                return BaseResponse<bool>.FailResponse("Contact your support service.");
            }
        }

        public async Task<BaseResponse<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var product = await productRepository.GetProductByIdAsync (id, CancellationToken.None);

                if ((product == null))
                {
                    return BaseResponse<bool>.FailResponse("Product not found.");
                }
                var result = await productRepository.DeleteAsync(product, CancellationToken.None);

                if ((!result))
                {
                    return BaseResponse<bool>.FailResponse("Failed to delete product.");
                }
                return BaseResponse<bool>.SuccessResponse(true, "Product deleted successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while deleting product.");
                return BaseResponse<bool>.FailResponse($"Error: {ex.Message}");
            }
        }

        public async Task<BaseResponse<IEnumerable<ProductDto>>> GetProductsByCategoryIdAsync(Guid categoryId)
        {
            try
            {
                var products = await productRepository.GetProductsAsync(CancellationToken.None);
                if (products == null || !products.Any())
                    return BaseResponse<IEnumerable<ProductDto>>.FailResponse("No products found.");
                
                var filteredProducts = products
                      .Where(p => p.CategoryId == categoryId)
                      .ToList();
                if (!filteredProducts.Any())
                    return BaseResponse<IEnumerable<ProductDto>>.FailResponse("No products found for the specified category.");
                
                var productDtos = filteredProducts.Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.SellingPrice,
                    CostPrice = p.CostPrice,
                    Description = p.Description,
                    Category = p.Category != null ? p.Category.Name : string.Empty,
                    ExpiryDate = p.ExpiryDate,
                    DateCreated = p.CreatedAt,
                    StockQuantity = p.StockQuantity
                }).ToList();

                return BaseResponse<IEnumerable<ProductDto>>.SuccessResponse(productDtos, "Products retrieved successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while fetching products.");
                return BaseResponse<IEnumerable<ProductDto>>.FailResponse($"Error: {ex.Message}");
            }
        }

        public async Task<BaseResponse<ProductDto>> GetByIdAsync(Guid id)
        {
            try
            {
                var product = await productRepository.GetProductByIdAsync(id, CancellationToken.None);

                if ((product == null))
                {
                    return BaseResponse<ProductDto>.FailResponse("Product not found.");
                }
                var productDto = new ProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.SellingPrice,
                    CostPrice = product.CostPrice,
                    Description = product.Description,
                    Category = product.Category != null ? product.Category.Name : "Uncategorized",
                    ExpiryDate = product.ExpiryDate,
                    DateCreated = product.CreatedAt,
                    StockQuantity = product.StockQuantity
                };

                return BaseResponse<ProductDto>.SuccessResponse(productDto, "Product retrieved successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while fetching product by ID.");
                return BaseResponse<ProductDto>.FailResponse($"Error: {ex.Message}");
            }
        }

        public async Task<BaseResponse<bool>> UpdateAsync(Guid id, CreateProductDto request)
        {
            try
            {
                var product = await productRepository.GetProductByIdAsync(id, CancellationToken.None);

                if (product == null)
                    return BaseResponse<bool>.FailResponse("Product not found.");

                // Update product fields
                product.Name = request.Name;
                product.Description = request.Description;
                product.SellingPrice = request.SellingPrice;
                product.CostPrice = request.CostPrice;
                product.StockQuantity = request.StockQuantity;
                product.CategoryId = request.CategoryId;
                if (request.ExpiryDate.HasValue)
                    product.ExpiryDate = request.ExpiryDate.Value;
                product.UpdatedAt = DateTime.UtcNow;

                var result = await productRepository.UpdateAsync(product, CancellationToken.None);

                if (!result)
                    return BaseResponse<bool>.FailResponse("Failed to update product.");

                return BaseResponse<bool>.SuccessResponse(true, "Product updated successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while updating product.");
                return BaseResponse<bool>.FailResponse($"Error: {ex.Message}");
            }
        }

        
        public async Task<BaseResponse<IEnumerable<ProductDto>>> GetAllAsync()
        {
            try
            {
                var products = await productRepository.GetProductsAsync(CancellationToken.None);

                if (products == null || !products.Any())
                    return BaseResponse<IEnumerable<ProductDto>>.FailResponse("No products found.");

                var productDtos = products.Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.SellingPrice,
                    CostPrice = p.CostPrice,
                    StockQuantity = p.StockQuantity,
                    ExpiryDate = p.ExpiryDate,
                    DateCreated = p.CreatedAt,
                    Category = p.Category != null ? p.Category.Name : "Uncategorized"
                });

                return BaseResponse<IEnumerable<ProductDto>>.SuccessResponse(productDtos, "Products retrieved successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving all products.");
                return BaseResponse<IEnumerable<ProductDto>>.FailResponse("An error occurred while retrieving products.");
            }
        }
    }
}
