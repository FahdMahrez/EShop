using EShop.Data;
using EShop.Dto;
using EShop.Dto.CategoryModel;
using EShop.Repositories.Interface;
using EShop.Services.Interface;

namespace EShop.Services
{
    public class CategoryService(ICategoryRepository categoryRepository, ILogger<CategoryService> logger) : ICategoryService
    {
        public async Task<BaseResponse<bool>> CreateAsync(CreateCategoryDto request)
        {
            try
            {
                var category = new Category
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    Description = request.Description
                };

                var result = await categoryRepository.CreateAsync(category, CancellationToken.None);

                if ((!result))
                {
                    return BaseResponse<bool>.FailResponse("Failed to create category.");
                }
                return BaseResponse<bool>.SuccessResponse(true, "Category created successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while deleting category.");
                return BaseResponse<bool>.FailResponse($"Error: {ex.Message}");
            }
        }

        public async Task<BaseResponse<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var category = await categoryRepository.GetByIdAsync(id, CancellationToken.None);

                if ((category == null))
                {
                    return BaseResponse<bool>.FailResponse("Category not found.");
                }
                var result = await categoryRepository.DeleteAsync(category, CancellationToken.None);

                if ((!result))
                {
                    return BaseResponse<bool>.FailResponse("Failed to delete category.");
                }
                return BaseResponse<bool>.SuccessResponse(true, "Category deleted successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while deleting category.");
                return BaseResponse<bool>.FailResponse($"Error: {ex.Message}");
            }
        }

        public async Task<BaseResponse<IEnumerable<CategoryDto>>> GetAllAsync()
        {
            try
            {
                var categories = await categoryRepository.GetAllAsync(CancellationToken.None);

                if (categories == null || !categories.Any())
                    return BaseResponse<IEnumerable<CategoryDto>>.FailResponse("No categories found.");

                var categoryDtos = categories.Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description
                });

                return BaseResponse<IEnumerable<CategoryDto>>.SuccessResponse(categoryDtos, "Categories retrieved successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error retrieving categories: {ex.Message}");
                return BaseResponse<IEnumerable<CategoryDto>>.FailResponse("An error occurred while retrieving categories.");
            }
        }

        public async Task<BaseResponse<CategoryDto>> GetByIdAsync(Guid id)
        {
            try
            {
                var category = await categoryRepository.GetByIdAsync(id, CancellationToken.None);

                if (category == null)
                    return BaseResponse<CategoryDto>.FailResponse("Category not found.");

                var categoryDto = new CategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description
                };

                return BaseResponse<CategoryDto>.SuccessResponse(categoryDto, "Category retrieved successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error retrieving category by ID: {ex.Message}");
                return BaseResponse<CategoryDto>.FailResponse("An error occurred while retrieving the category.");
            }
        }

        public async Task<BaseResponse<bool>> UpdateAsync(Guid id, CreateCategoryDto request)
        {
            try
            {
                var category = await categoryRepository.GetByIdAsync(id, CancellationToken.None);

                if ((category == null))
                {
                    return BaseResponse<bool>.FailResponse("Category not found.");
                }
                category.Name = request.Name;
                category.Description = request.Description;
                category.UpdatedAt = DateTime.UtcNow;

                var result = await categoryRepository.UpdateAsync(category, CancellationToken.None);

                if ((!result))
                {
                    return BaseResponse<bool>.FailResponse("Failed to update category.");
                }
                return BaseResponse<bool>.SuccessResponse(true, "Category updated successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while updating category.");
                return BaseResponse<bool>.FailResponse($"Error: {ex.Message}");
            }
        }
    }
}