using EShop.Data;

namespace EShop.Repositories.Interface
{
    public interface IUserRepository
    {
        Task<bool> CreateAsync(User user, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(User user, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(User user, CancellationToken cancellationToken);
        Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
        Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken);
        Task<bool> UpdateRefreshTokenAsync(Guid userId, string refreshToken, DateTime expiry, CancellationToken cancellationToken);
    }
}
