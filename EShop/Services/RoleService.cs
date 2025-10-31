using EShop.Data;
using EShop.Dto;
using EShop.Dto.RoleModel;
using EShop.Repositories.Interface;
using EShop.Services.Interface;
using Serilog;

namespace EShop.Services
{
    public class RoleService(IRoleRepository roleRepository) : IRoleService
    {
        public async Task<BaseResponse<RoleDto>> CreateAsync(CreateRoleDto request)
        {
            try
            {
                Log.Information("Creating a new role with name: {RoleName}", request.Name);

                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    Log.Warning("Attempt to create role with empty name");
                    return BaseResponse<RoleDto>.FailResponse("Role name cannot be empty.");
                }
                var existing = await roleRepository.GetByNameAsync(request.Name, CancellationToken.None);
                if (existing != null)
                {
                    Log.Warning("Role '{RoleName}' already exists", request.Name);
                    return BaseResponse<RoleDto>.FailResponse("Role already exists.");
                }
                var role = new Role
                {
                    Id = Guid.NewGuid(),
                    RoleName = request.Name,
                    Description = request.Description
                };

                var success = await roleRepository.CreateAsync(role, CancellationToken.None);
                if (!success)
                {
                    Log.Error("Failed to create role '{RoleName}'", request.Name);
                    return BaseResponse<RoleDto>.FailResponse("Failed to create role.");
                }
                Log.Information("Role '{RoleName}' created successfully", request.Name);
                var roleDto = new RoleDto
                {
                    Id = role.Id,
                    Name = role.RoleName,
                    Description = role.Description
                };

                return BaseResponse<RoleDto>.SuccessResponse(roleDto, "Role created successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while creating role '{RoleName}'", request.Name);
                return BaseResponse<RoleDto>.FailResponse("An error occurred while creating the role.");
            }
        }

        public async Task<BaseResponse<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var role = await roleRepository.GetByIdAsync(id, CancellationToken.None);
                if (role == null)
                {
                    Log.Warning("Attempt to delete non-existent role with ID {RoleId}", id);
                    return BaseResponse<bool>.FailResponse("Role not found.");
                }
                var success = await roleRepository.DeleteAsync(role, CancellationToken.None);
                if (!success)
                {
                    Log.Error("Failed to delete role with ID {RoleId}", id);
                    return BaseResponse<bool>.FailResponse("Failed to delete role.");
                }
                Log.Information("Role with ID {RoleId} deleted successfully", id);
                return BaseResponse<bool>.SuccessResponse(true, "Role deleted successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while deleting role with ID {RoleId}", id);
                return BaseResponse<bool>.FailResponse("An error occurred while deleting the role.");
            }
        }

        public async Task<BaseResponse<IEnumerable<RoleDto>>> GetAllAsync()
        {
            try
            {
                var roles = await roleRepository.GetAllAsync(CancellationToken.None);
                if (roles == null || !roles.Any())
                {
                    Log.Information("No roles found in database");
                    return BaseResponse<IEnumerable<RoleDto>>.FailResponse("No roles found.");
                }
                Log.Information("{Count} roles retrieved successfully", roles.Count());
                var roleDtos = roles.Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.RoleName,
                    Description = r.Description
                });

                return BaseResponse<IEnumerable<RoleDto>>.SuccessResponse(roleDtos, "Roles retrieved successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while retrieving all roles");
                return BaseResponse<IEnumerable<RoleDto>>.FailResponse("An error occurred while retrieving roles.");
            }
        }

        public async  Task<BaseResponse<RoleDto>> GetByIdAsync(Guid id)
        {
            try
            {
                var role = await roleRepository.GetByIdAsync(id, CancellationToken.None);
                if (role == null)
                {
                    Log.Warning("Role with ID {RoleId} not found", id);
                    return BaseResponse<RoleDto>.FailResponse("Role not found.");
                }
                Log.Information("Role '{RoleName}' with ID {RoleId} retrieved successfully", role.RoleName, id);
                var roleDto = new RoleDto
                {
                    Id = role.Id,
                    Name = role.RoleName,
                    Description = role.Description
                };

                return BaseResponse<RoleDto>.SuccessResponse(roleDto, "Role retrieved successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while fetching role with ID {RoleId}", id);
                return BaseResponse<RoleDto>.FailResponse("An error occurred while fetching the role.");
            }
        }

        public async Task<BaseResponse<RoleDto>> GetByNameAsync(string name)
        {
            try
            {
                var role = await roleRepository.GetByNameAsync(name, CancellationToken.None);
                if (role == null)
                {
                    Log.Warning("Role '{RoleName}' not found", name);
                    return BaseResponse<RoleDto>.FailResponse("Role not found.");
                }
                Log.Information("Role '{RoleName}' retrieved successfully", name);
                var roleDto = new RoleDto
                {
                    Id = role.Id,
                    Name = role.RoleName,
                    Description = role.Description
                };

                return BaseResponse<RoleDto>.SuccessResponse(roleDto, "Role retrieved successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while fetching role by name '{RoleName}'", name);
                return BaseResponse<RoleDto>.FailResponse("An error occurred while fetching the role.");
            }
        }

        public async Task<BaseResponse<bool>> UpdateAsync(Guid id, CreateRoleDto request)
        {
            try
            {
                var role = await roleRepository.GetByIdAsync(id, CancellationToken.None);
                if (role == null)
                {
                    Log.Warning("Attempted to update non-existent role with ID {RoleId}", id);
                    return BaseResponse<bool>.FailResponse("Role not found.");
                }
                role.RoleName = request.Name;
                role.Description = request.Description;

                var success = await roleRepository.UpdateAsync(role, CancellationToken.None);
                if (!success)
                {
                    Log.Error("Failed to update role with ID {RoleId}", id);
                    return BaseResponse<bool>.FailResponse("Failed to update role.");
                }
                Log.Information("Role '{RoleName}' with ID {RoleId} updated successfully", role.RoleName, id);
                return BaseResponse<bool>.SuccessResponse(true, "Role updated successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while updating role with ID {RoleId}", id);
                return BaseResponse<bool>.FailResponse("An error occurred while updating the role.");
            }
        }

        Task<BaseResponse<RoleDto>> IRoleService.UpdateAsync(Guid id, CreateRoleDto request)
        {
            throw new NotImplementedException();
        }
    }
}
