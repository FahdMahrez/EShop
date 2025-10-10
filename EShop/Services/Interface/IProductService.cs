using EShop.Dto;
using EShop.Dto.ProductModel;

namespace EShop.Services.Interface
{
    public interface IProductService
    {
        Task<BaseResponse<bool>> CreateAsync(CreateProductDto request);
        Task<BaseResponse<bool>> UpdateAsync(Guid id, CreateProductDto request);
        Task<BaseResponse<bool>> DeleteAsync(Guid id);
        Task<BaseResponse<ProductDto>> GetByIdAsync(Guid id);
        Task<BaseResponse<IEnumerable<ProductDto>>> GetProductsByCategoryIdAsync(Guid categoryId);
        Task<BaseResponse<IEnumerable<ProductDto>>> GetAllAsync();
    }
}

