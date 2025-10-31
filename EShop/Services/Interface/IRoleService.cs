using EShop.Dto;
using EShop.Dto.RoleModel;

namespace EShop.Services.Interface
{
    public interface IRoleService
    {
        Task<BaseResponse<RoleDto>> CreateAsync(CreateRoleDto request);
        Task<BaseResponse<IEnumerable<RoleDto>>> GetAllAsync();
        Task<BaseResponse<RoleDto>> GetByIdAsync(Guid id);
        Task<BaseResponse<RoleDto>> GetByNameAsync(string name);
        Task<BaseResponse<bool>> DeleteAsync(Guid id);
        Task<BaseResponse<RoleDto>> UpdateAsync(Guid id, CreateRoleDto request);
    }
}
