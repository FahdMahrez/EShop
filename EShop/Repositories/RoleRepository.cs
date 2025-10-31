using EShop.Context;
using EShop.Data;
using EShop.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace EShop.Repositories
{
    public class RoleRepository(ApplicationDbContext dbContext) : IRoleRepository
    {
        public async Task<bool> CreateAsync(Role role, CancellationToken cancellationToken)
        {
            await dbContext.Roles.AddAsync(role, cancellationToken);
            return await dbContext.SaveChangesAsync(cancellationToken) > 0;
        }

        public async Task<bool> DeleteAsync(Role role, CancellationToken cancellationToken)
        {
            dbContext.Roles.Remove(role);
            return await dbContext.SaveChangesAsync(cancellationToken) > 0;
        }

        public async Task<IEnumerable<Role>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await dbContext.Roles
                            .AsNoTracking()
                            .ToListAsync(cancellationToken);
        }

        public async Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await dbContext.Roles
                            .AsNoTracking()
                            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        }

        public async Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken)
        {
            return await dbContext.Roles
                            .AsNoTracking()
                            .FirstOrDefaultAsync(r => r.RoleName.ToLower() == name.ToLower(), cancellationToken);
        }

        public async Task<bool> UpdateAsync(Role role, CancellationToken cancellationToken)
        {
            dbContext.Roles.Update(role);
            return await dbContext.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}
