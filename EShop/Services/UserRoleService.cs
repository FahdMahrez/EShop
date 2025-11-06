using EShop.Dto;
using EShop.Repositories.Interface;
using EShop.Services.Interface;
using Serilog;

namespace EShop.Services
{
    public class UserRoleService(IUserRoleRepository userRoleRepository) : IUserRoleService
    {

        public async Task<BaseResponse<bool>> AssignRoleToUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken)
        {
            try
            {
                Log.Information("Attempting to assign RoleId {RoleId} to UserId {UserId}", roleId, userId);

                var success = await userRoleRepository.AssignRoleToUserAsync(userId, roleId, CancellationToken.None);
                if (!success)
                {
                    Log.Warning("Failed to assign RoleId {RoleId} to UserId {UserId}. User might already have this role.", roleId, userId);
                    return BaseResponse<bool>.FailResponse("User already has this role or assignment failed.");
                }
                Log.Information("Successfully assigned RoleId {RoleId} to UserId {UserId}", roleId, userId);
                return BaseResponse<bool>.SuccessResponse(true, "Role assigned to user successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while assigning RoleId {RoleId} to UserId {UserId}", roleId, userId);
                return BaseResponse<bool>.FailResponse("An error occurred while assigning role to user.");
            }
        }

        public async Task<BaseResponse<IEnumerable<string>>> GetRolesByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
try
            {
                Log.Information("Retrieving roles for UserId {UserId}", userId);

                var roles = await userRoleRepository.GetRolesByUserIdAsync(userId, CancellationToken.None);
                if (roles == null || !roles.Any())
                {
                    Log.Warning("No roles found for UserId {UserId}", userId);
                    return BaseResponse<IEnumerable<string>>.FailResponse("No roles found for this user.");
                }
                var roleNames = roles.Select(r => r.RoleName);
                return BaseResponse<IEnumerable<string>>.SuccessResponse(roleNames, "Roles retrieved successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while retrieving roles for UserId {UserId}", userId);
                return BaseResponse<IEnumerable<string>>.FailResponse("An error occurred while fetching roles.");
            }        }

        public async Task<BaseResponse<bool>> RemoveRoleFromUserAsync(Guid userId, Guid roleId)
        {
            try
            {
                Log.Information("Attempting to remove RoleId {RoleId} from UserId {UserId}", roleId, userId);
                var success = await userRoleRepository.RemoveRoleFromUserAsync(userId, roleId, CancellationToken.None);
                if (!success)
                {
                    Log.Warning("Failed to remove RoleId {RoleId} from UserId {UserId}. User may not have this role.", roleId, userId);
                    return BaseResponse<bool>.FailResponse("Role removal failed or user did not have this role.");
                }
                Log.Information("Successfully removed RoleId {RoleId} from UserId {UserId}", roleId, userId);
                return BaseResponse<bool>.SuccessResponse(true, "Role removed from user successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while removing RoleId {RoleId} from UserId {UserId}", roleId, userId);
                return BaseResponse<bool>.FailResponse("An error occurred while removing role from user.");
            }
        }
    }
}
