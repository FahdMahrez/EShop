using EShop.Data;
using EShop.Dto;
using EShop.Dto.CategoryModel;
using EShop.Repositories.Interface;
using EShop.Services.Interface;
using Serilog;

namespace EShop.Services
{
    public class CategoryService(ICategoryRepository categoryRepository) : ICategoryService
    {
        public async Task<BaseResponse<bool>> CreateAsync(CreateCategoryDto request)
        {
            try
            {
                Log.Information("Creating a new category with name: {CategoryName}", request.Name);

                var category = new Category
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    Description = request.Description
                };

                var result = await categoryRepository.CreateAsync(category, CancellationToken.None);

                if ((!result))
                {
                    Log.Warning("Failed to create category: {CategoryName}", request.Name);
                    return BaseResponse<bool>.FailResponse("Failed to create category.");
                }
                Log.Information("Category created successfully: {@Category}", category);
                return BaseResponse<bool>.SuccessResponse(true, "Category created successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while creating category: {CategoryName}", request.Name);
                return BaseResponse<bool>.FailResponse($"Error: {ex.Message}"); ;
            }
        }

        public async Task<BaseResponse<bool>> DeleteAsync(Guid id)
        {
            try
            {
                Log.Information("Attempting to delete category with ID: {CategoryId}", id);

                var category = await categoryRepository.GetByIdAsync(id, CancellationToken.None);

                if ((category == null))
                {
                    Log.Warning("Category not found: {CategoryId}", id);
                    return BaseResponse<bool>.FailResponse("Category not found.");
                }
                var result = await categoryRepository.DeleteAsync(category, CancellationToken.None);

                if ((!result))
                {
                    Log.Warning("Failed to delete category: {CategoryId}", id);
                    return BaseResponse<bool>.FailResponse("Failed to delete category.");
                }
                Log.Information("Category deleted successfully: {CategoryId}", id);
                return BaseResponse<bool>.SuccessResponse(true, "Category deleted successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while deleting category: {CategoryId}", id);
                return BaseResponse<bool>.FailResponse($"Error: {ex.Message}");
            }
        }

        public async Task<BaseResponse<IEnumerable<CategoryDto>>> GetAllAsync()
        {
            try
            {
                Log.Information("Fetching all categories...");

                var categories = await categoryRepository.GetAllAsync(CancellationToken.None);

                if (categories == null || !categories.Any())
                {
                    Log.Warning("No categories found in database.");
                    return BaseResponse<IEnumerable<CategoryDto>>.FailResponse("No categories found.");
                }
                var categoryDtos = categories.Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description
                });

                Log.Warning("No categories found in database.");
                return BaseResponse<IEnumerable<CategoryDto>>.SuccessResponse(categoryDtos, "Categories retrieved successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while retrieving all categories.");
                return BaseResponse<IEnumerable<CategoryDto>>.FailResponse("An error occurred while retrieving categories.");
            }
        }

        public async Task<BaseResponse<CategoryDto>> GetByIdAsync(Guid id)
        {
            try
            {
                Log.Information("Fetching category by ID: {CategoryId}", id);

                var category = await categoryRepository.GetByIdAsync(id, CancellationToken.None);

                if (category == null)
                {
                    Log.Warning("Category not found: {CategoryId}", id);
                    return BaseResponse<CategoryDto>.FailResponse("Category not found.");
                }
                var categoryDto = new CategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description
                };
                Log.Information("Category retrieved successfully: {@CategoryDto}", categoryDto);
                return BaseResponse<CategoryDto>.SuccessResponse(categoryDto, "Category retrieved successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while fetching category by ID: {CategoryId}", id);
                return BaseResponse<CategoryDto>.FailResponse("An error occurred while retrieving the category.");
            }
        }

        public async Task<BaseResponse<bool>> UpdateAsync(Guid id, CreateCategoryDto request)
        {
            try
            {
                Log.Information("Updating category with ID: {CategoryId}", id);

                var category = await categoryRepository.GetByIdAsync(id, CancellationToken.None);

                if ((category == null))
                {
                    Log.Warning("Category not found: {CategoryId}", id);
                    return BaseResponse<bool>.FailResponse("Category not found.");
                }
                category.Name = request.Name;
                category.Description = request.Description;
                category.UpdatedAt = DateTime.UtcNow;

                var result = await categoryRepository.UpdateAsync(category, CancellationToken.None);

                if ((!result))
                {
                    Log.Warning("Failed to update category: {CategoryId}", id);
                    return BaseResponse<bool>.FailResponse("Failed to update category.");
                }
                Log.Information("Category updated successfully: {@Category}", category);
                return BaseResponse<bool>.SuccessResponse(true, "Category updated successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while updating category: {CategoryId}", id);
                return BaseResponse<bool>.FailResponse($"Error: {ex.Message}");
            }
        }
    }
}