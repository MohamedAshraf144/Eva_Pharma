using BookShop.DataAccess.Repository.IRepository;
using BookShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookShop.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private const int PageSize = 8;

        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Product
        public async Task<IActionResult> Index(int page = 1, int? categoryFilter = null, string? search = null)
        {
            var products = await _unitOfWork.Product.GetAllAsync(includeProperties: "Category");

            // تطبيق البحث
            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(p =>
                    p.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    p.Description.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    p.Author.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    p.Category.CatName.Contains(search, StringComparison.OrdinalIgnoreCase));
                ViewBag.SearchTerm = search;
            }

            // تطبيق فلتر الكاتيجوري
            if (categoryFilter.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryFilter.Value);
                ViewBag.CategoryFilter = categoryFilter;
            }

            // تطبيق Pagination
            var productsList = products.ToList();
            var totalProducts = productsList.Count;
            var paginatedProducts = productsList
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            // إعداد ViewBag
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalProducts / PageSize);
            ViewBag.HasPreviousPage = page > 1;
            ViewBag.HasNextPage = page < ViewBag.TotalPages;
            ViewBag.CategoryList = await GetCategorySelectListAsync();

            return View(paginatedProducts);
        }

        // GET: Product/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.CategoryList = await GetCategorySelectListAsync();
            return View();
        }

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Add(product);
                await _unitOfWork.SaveAsync();

                TempData["Success"] = "Product created successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.CategoryList = await GetCategorySelectListAsync();
            return View(product);
        }

        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _unitOfWork.Product.GetAsync(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.CategoryList = await GetCategorySelectListAsync();
            return View(product);
        }

        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _unitOfWork.Product.Update(product);
                    await _unitOfWork.SaveAsync();

                    TempData["Success"] = "Product updated successfully!";
                }
                catch (Exception)
                {
                    if (!await ProductExistsAsync(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.CategoryList = await GetCategorySelectListAsync();
            return View(product);
        }

        // GET: Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _unitOfWork.Product.GetAsync(p => p.Id == id, includeProperties: "Category");
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _unitOfWork.Product.GetAsync(p => p.Id == id);
            if (product != null)
            {
                _unitOfWork.Product.Remove(product);
                await _unitOfWork.SaveAsync();

                TempData["Success"] = "Product deleted successfully!";
            }

            return Json(new { success = true });
        }

        private async Task<IEnumerable<SelectListItem>> GetCategorySelectListAsync()
        {
            var categories = await _unitOfWork.Category.GetAllAsync(filter: c => !c.IsDeleted && c.IsActive);
            var selectList = categories.Select(c => new SelectListItem
            {
                Text = c.CatName,
                Value = c.Id.ToString()
            }).ToList();

            // إضافة خيار "All Categories" في البداية للفلتر
            selectList.Insert(0, new SelectListItem { Text = "All Categories", Value = "" });

            return selectList;
        }

        private async Task<bool> ProductExistsAsync(int id)
        {
            var product = await _unitOfWork.Product.GetAsync(p => p.Id == id);
            return product != null;
        }
    }
}