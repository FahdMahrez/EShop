using EShop.Data;
using EShop.Dto;
using EShop.Dto.ProductModel;
using EShop.Repositories;
using EShop.Repositories.Interface;
using EShop.Services.Interface;
using Serilog;

namespace EShop.Services
{
    public class ProductService(IProductRepository productRepository) : IProductService
    {
        public async Task<BaseResponse<bool>> CreateAsync(CreateProductDto request)
        {
            try
            {
                if ((request == null))
                {
                    Log.Warning("Attempted to create a product with null request data.");
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
                    Log.Warning("Failed to create product {ProductName}", request.Name);
                    return BaseResponse<bool>.FailResponse("Failed to create product.");
                }
                Log.Information("Product created successfully: {@Product}", product);
                return BaseResponse<bool>.SuccessResponse(true, "Product created successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while creating product {ProductName}", request?.Name);
                return BaseResponse<bool>.FailResponse("Contact your support service.");
            }
        }

        public async Task<BaseResponse<bool>> DeleteAsync(Guid id)
        {
            try
            {
                Log.Information("Attempting to delete product with ID: {ProductId}", id);

                var product = await productRepository.GetProductByIdAsync (id, CancellationToken.None);

                if ((product == null))
                {
                    Log.Warning("Product not found for deletion: {ProductId}", id);
                    return BaseResponse<bool>.FailResponse("Product not found.");
                }
                var result = await productRepository.DeleteAsync(product, CancellationToken.None);

                if ((!result))
                {
                    Log.Warning("Failed to delete product: {ProductId}", id);
                    return BaseResponse<bool>.FailResponse("Failed to delete product.");
                }
                Log.Information("Product deleted successfully: {ProductId}", id);
                return BaseResponse<bool>.SuccessResponse(true, "Product deleted successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while deleting product: {ProductId}", id);
                return BaseResponse<bool>.FailResponse($"Error: {ex.Message}");
            }
        }

        public async Task<BaseResponse<IEnumerable<ProductDto>>> GetProductsByCategoryIdAsync(Guid categoryId)
        {
            try
            {
                Log.Information("Fetching products by Category ID: {CategoryId}", categoryId);

                var products = await productRepository.GetProductsAsync(CancellationToken.None);
                if (products == null || !products.Any())
                    return BaseResponse<IEnumerable<ProductDto>>.FailResponse("No products found.");
                
                var filteredProducts = products
                      .Where(p => p.CategoryId == categoryId)
                      .ToList();
                if (!filteredProducts.Any())
                {
                    Log.Warning("No products found for category: {CategoryId}", categoryId);
                    return BaseResponse<IEnumerable<ProductDto>>.FailResponse("No products found for the specified category.");
                }
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
                Log.Information("Retrieved {Count} products for category: {CategoryId}", productDtos.Count, categoryId);
                return BaseResponse<IEnumerable<ProductDto>>.SuccessResponse(productDtos, "Products retrieved successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while fetching products for category: {CategoryId}", categoryId);
                return BaseResponse<IEnumerable<ProductDto>>.FailResponse($"Error: {ex.Message}");
            }
        }

        public async Task<BaseResponse<ProductDto>> GetByIdAsync(Guid id)
        {
            try
            {
                Log.Information("Fetching product by ID: {ProductId}", id);

                var product = await productRepository.GetProductByIdAsync(id, CancellationToken.None);

                if ((product == null))
                {
                    Log.Warning("Product not found with ID: {ProductId}", id);
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
                Log.Information("Product retrieved successfully: {@ProductDto}", productDto);
                return BaseResponse<ProductDto>.SuccessResponse(productDto, "Product retrieved successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while fetching product by ID: {ProductId}", id);
                return BaseResponse<ProductDto>.FailResponse($"Error: {ex.Message}");
            }
        }

        public async Task<BaseResponse<bool>> UpdateAsync(Guid id, CreateProductDto request)
        {
            try
            {
                Log.Information("Attempting to update product with ID: {ProductId}", id);

                var product = await productRepository.GetProductByIdAsync(id, CancellationToken.None);

                if (product == null)
                {
                    Log.Warning("Product not found for update: {ProductId}", id);
                    return BaseResponse<bool>.FailResponse("Product not found.");
                }
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
                Log.Error(ex, "Error occurred while updating product: {ProductId}", id);
                return BaseResponse<bool>.FailResponse($"Error: {ex.Message}");
            }
        }

        
        public async Task<BaseResponse<IEnumerable<ProductDto>>> GetAllAsync()
        {
            try
            {
                Log.Information("Fetching all products...");

                var products = await productRepository.GetProductsAsync(CancellationToken.None);

                if (products == null || !products.Any())
                {
                    Log.Warning("No products found in database.");
                    return BaseResponse<IEnumerable<ProductDto>>.FailResponse("No products found.");
                }
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
                Log.Information("Retrieved {Count} products successfully.", productDtos.Count());
                return BaseResponse<IEnumerable<ProductDto>>.SuccessResponse(productDtos, "Products retrieved successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while retrieving all products.");
                return BaseResponse<IEnumerable<ProductDto>>.FailResponse("An error occurred while retrieving products.");
            }
        }
    }
}
