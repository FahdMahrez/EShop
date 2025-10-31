using EShop.Data;

namespace EShop.Repositories.Interface
{
    public interface IUserRoleRepository
    {
            Task<bool> AssignRoleToUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken);
            Task<IEnumerable<Role>> GetRolesByUserIdAsync(Guid userId, CancellationToken cancellationToken);
            Task<bool> RemoveRoleFromUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken);
    }
}
