using BookShop.DataAccess.Repository.IRepository;
using BookShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookShop.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProductController> _logger;
        private const int PageSize = 8;

        public ProductController(IUnitOfWork unitOfWork, ILogger<ProductController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // GET: Product/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                _logger.LogInformation("Create GET action called");

                var categoryList = await GetCategorySelectListAsync();
                _logger.LogInformation($"Categories loaded: {categoryList.Count()}");

                ViewBag.CategoryList = categoryList;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Create GET action");
                TempData["Error"] = "Error loading create form: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            try
            {
                _logger.LogInformation("Create POST action called");
                _logger.LogInformation($"Product data: {System.Text.Json.JsonSerializer.Serialize(product)}");

                // إزالة الـ Category من الـ ModelState لأننا مش محتاجينها
                ModelState.Remove("Category");

                if (ModelState.IsValid)
                {
                    _logger.LogInformation("Model state is valid");

                    // تأكد من إن الـ Category موجودة
                    var categoryExists = await _unitOfWork.Category.GetAsync(c => c.Id == product.CategoryId && !c.IsDeleted && c.IsActive);
                    if (categoryExists == null)
                    {
                        ModelState.AddModelError("CategoryId", "Selected category is not valid");
                        ViewBag.CategoryList = await GetCategorySelectListAsync();
                        return View(product);
                    }

                    _unitOfWork.Product.Add(product);
                    await _unitOfWork.SaveAsync();

                    _logger.LogInformation($"Product created with ID: {product.Id}");
                    TempData["Success"] = "Product created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _logger.LogWarning("Model state is invalid");
                    foreach (var error in ModelState)
                    {
                        _logger.LogWarning($"Field: {error.Key}, Errors: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
                    }
                }

                ViewBag.CategoryList = await GetCategorySelectListAsync();
                return View(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Create POST action");
                TempData["Error"] = "Error creating product: " + ex.Message;
                ViewBag.CategoryList = await GetCategorySelectListAsync();
                return View(product);
            }
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

            // إزالة الـ Category من الـ ModelState
            ModelState.Remove("Category");

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

        public async Task<IActionResult> Index(int page = 1, int? categoryFilter = null, string? search = null)
        {
            var products = await _unitOfWork.Product.GetAllAsync(includeProperties: "Category");

            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(p =>
                    p.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    p.Description.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    p.Author.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    p.Category.CatName.Contains(search, StringComparison.OrdinalIgnoreCase));
                ViewBag.SearchTerm = search;
            }

            if (categoryFilter.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryFilter.Value);
                ViewBag.CategoryFilter = categoryFilter;
            }

            var productsList = products.ToList();
            var totalProducts = productsList.Count;
            var paginatedProducts = productsList
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalProducts / PageSize);
            ViewBag.HasPreviousPage = page > 1;
            ViewBag.HasNextPage = page < ViewBag.TotalPages;
            ViewBag.CategoryList = await GetCategorySelectListAsync();

            return View(paginatedProducts);
        }

        private async Task<IEnumerable<SelectListItem>> GetCategorySelectListAsync()
        {
            try
            {
                var categories = await _unitOfWork.Category.GetAllAsync(filter: c => !c.IsDeleted && c.IsActive);
                _logger.LogInformation($"Found {categories.Count()} active categories");

                var selectList = categories.Select(c => new SelectListItem
                {
                    Text = c.CatName,
                    Value = c.Id.ToString()
                }).ToList();

                return selectList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading categories");
                return new List<SelectListItem>();
            }
        }

        private async Task<bool> ProductExistsAsync(int id)
        {
            var product = await _unitOfWork.Product.GetAsync(p => p.Id == id);
            return product != null;
        }
    }
}