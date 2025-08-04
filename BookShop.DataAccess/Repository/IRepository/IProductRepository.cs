using BookShop.Models;

namespace BookShop.DataAccess.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
        Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);
        Task<IEnumerable<Product>> GetTopSellingProductsAsync(int count = 10);
        Task<bool> IsProductTitleExistsAsync(string title, int? excludeId = null);
    }
}