using EShop.Context;
using EShop.Data;
using EShop.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace EShop.Repositories
{
    public class UserRepository(ApplicationDbContext dbContext) : IUserRepository
    {
        public async Task<bool> CreateAsync(User user, CancellationToken cancellationToken)
        {
            await dbContext.Users.AddAsync(user, cancellationToken);
            return await dbContext.SaveChangesAsync(cancellationToken) > 0;
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

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            Log.Information("Fetching user by email {Email}", email);
            return await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await dbContext.Users
                            .AsNoTracking()
                            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        public async Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            Log.Information("Fetching user by refresh token...");
            return await dbContext.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken, cancellationToken);
        }

        public async Task<bool> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            dbContext.Users.Update(user);
            return await dbContext.SaveChangesAsync(cancellationToken) > 0;
        }
        async Task<bool> IUserRepository.UpdateRefreshTokenAsync(Guid userId, string refreshToken, DateTime expiry, CancellationToken cancellationToken)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            if (user == null)
            {
                Log.Warning("User with ID {UserId} not found while updating refresh token", userId);
                return false;
            }

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = expiry;

            dbContext.Users.Update(user);
            return await dbContext.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}
