using BookShop.DataAccess.Repository.IRepository;
using BookShop.Models;
using Microsoft.EntityFrameworkCore;

namespace BookShop.DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetActiveCategoriesAsync()
        {
            return await _context.Categories
                .Where(c => !c.IsDeleted && c.IsActive)
                .OrderBy(c => c.CatOrder)
                .ThenBy(c => c.CatName)
                .ToListAsync();
        }

        public async Task<bool> IsCategoryNameExistsAsync(string categoryName, int? excludeId = null)
        {
            var query = _context.Categories.Where(c => !c.IsDeleted && c.CatName.ToLower() == categoryName.ToLower());

            if (excludeId.HasValue)
            {
                query = query.Where(c => c.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<int> GetProductsCountByCategoryAsync(int categoryId)
        {
            return await _context.Products.CountAsync(p => p.CategoryId == categoryId);
        }
    }
}