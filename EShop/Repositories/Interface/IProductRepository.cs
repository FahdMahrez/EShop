using EShop.Data;

namespace EShop.Repositories.Interface
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetProductsAsync(CancellationToken cancellationToken);
        Task<Product?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<bool> AddAsync(Product product, CancellationToken cancellationToken);
        Task<bool> UpdateAsync(Product product, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(Product product, CancellationToken cancellationToken);
    }
}
