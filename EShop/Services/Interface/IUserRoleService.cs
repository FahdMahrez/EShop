using EShop.Dto;

namespace EShop.Services.Interface
{
    public interface IUserRoleService
    {
        Task<BaseResponse<bool>> AssignRoleToUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken);
        Task<BaseResponse<IEnumerable<string>>> GetRolesByUserIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<BaseResponse<bool>> RemoveRoleFromUserAsync(Guid userId, Guid roleId);
    }
}
