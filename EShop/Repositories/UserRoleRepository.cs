using EShop.Context;
using EShop.Data;
using EShop.Repositories.Interface;
using Microsoft.EntityFrameworkCore;


namespace EShop.Repositories
{
    public class UserRoleRepository(ApplicationDbContext dbContext) : IUserRoleRepository
    {
        public async Task<bool> AssignRoleToUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken)
        {
            var exists = await dbContext.UserRoles.AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId, cancellationToken);
            if (exists) return false;

            var userRole = new UserRole { UserId = userId, RoleId = roleId };
            dbContext.UserRoles.Add(userRole);
            return await dbContext.SaveChangesAsync(cancellationToken) > 0;
        }

        public async Task<IEnumerable<Role>> GetRolesByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await dbContext.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.Role)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> RemoveRoleFromUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken)
        {
            var userRole = await dbContext.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId, cancellationToken);

            if (userRole == null) return false;

            dbContext.UserRoles.Remove(userRole);
            return await dbContext.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}
