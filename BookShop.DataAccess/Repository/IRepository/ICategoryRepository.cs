using BookShop.Models;

namespace BookShop.DataAccess.Repository.IRepository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<IEnumerable<Category>> GetActiveCategoriesAsync();
        Task<bool> IsCategoryNameExistsAsync(string categoryName, int? excludeId = null);
        Task<int> GetProductsCountByCategoryAsync(int categoryId);
    }
}