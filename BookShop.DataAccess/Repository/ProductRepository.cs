using BookShop.DataAccess.Repository.IRepository;
using BookShop.Models;
using Microsoft.EntityFrameworkCore;

namespace BookShop.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == categoryId)
                .OrderBy(p => p.Title)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
                return await _context.Products.Include(p => p.Category).ToListAsync();

            searchTerm = searchTerm.ToLower();
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.Title.ToLower().Contains(searchTerm) ||
                           p.Description.ToLower().Contains(searchTerm) ||
                           p.Author.ToLower().Contains(searchTerm) ||
                           p.Category.CatName.ToLower().Contains(searchTerm))
                .OrderBy(p => p.Title)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetTopSellingProductsAsync(int count = 10)
        {
            return await _context.Products
                .Include(p => p.Category)
                .OrderByDescending(p => p.Id)
                .Take(count)
                .ToListAsync();
        }

        public async Task<bool> IsProductTitleExistsAsync(string title, int? excludeId = null)
        {
            var query = _context.Products.Where(p => p.Title.ToLower() == title.ToLower());

            if (excludeId.HasValue)
            {
                query = query.Where(p => p.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }
    }
}