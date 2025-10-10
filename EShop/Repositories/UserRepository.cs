using EShop.Context;
using EShop.Data;
using EShop.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace EShop.Repositories
{
    public class UserRepository(ApplicationDbContext dbContext) : IUserRepository
    {
        public async Task<bool> CreateAsync(User user, CancellationToken cancellationToken)
         {  
            await dbContext.Users.AddAsync(user, cancellationToken);
            return await dbContext.SaveChangesAsync() > 0 ? true : false;
         }    

        public async Task<bool> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            dbContext.Users.Remove(user);
            return await dbContext.SaveChangesAsync(cancellationToken) > 0;
        }

        public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await dbContext.Users
                            .AsNoTracking()
                            .ToListAsync(cancellationToken);
        }

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await dbContext.Users
                            .AsNoTracking()
                            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        public async Task<bool> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            dbContext.Users.Update(user);
            return await dbContext.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}
