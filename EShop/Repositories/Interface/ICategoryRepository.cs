using EShop.Data;

namespace EShop.Repositories.Interface
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken);
        Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<bool> CreateAsync(Category category, CancellationToken cancellationToken);
        Task<bool> UpdateAsync(Category category, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(Category category, CancellationToken cancellationToken);
    }
}
