using BookShop.DataAccess.Repository.IRepository;
using BookShop.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.Web.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private const int PageSize = 10;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Category
        public async Task<IActionResult> GetAllCategories(int page = 1, string? search = null)
        {
            var categories = await _unitOfWork.Category.GetAllAsync(
                filter: c => !c.IsDeleted,
                orderBy: q => q.OrderBy(c => c.CatOrder).ThenBy(c => c.CatName)
            );

            // تطبيق البحث
            if (!string.IsNullOrEmpty(search))
            {
                categories = categories.Where(c =>
                    c.CatName.Contains(search, StringComparison.OrdinalIgnoreCase));
                ViewBag.CurrentSearch = search;
            }

            // تطبيق Pagination
            var categoriesList = categories.ToList();
            var totalCategories = categoriesList.Count;
            var paginatedCategories = categoriesList
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            // إعداد ViewBag للـ Pagination
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCategories / PageSize);
            ViewBag.HasPreviousPage = page > 1;
            ViewBag.HasNextPage = page < ViewBag.TotalPages;

            return View(paginatedCategories);
        }

        // GET: Category/Create
        public IActionResult CreateCategory()
        {
            return View();
        }

        // POST: Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                category.CreatedDate = DateTime.Now;
                category.IsDeleted = false;
                category.IsActive = true;

                _unitOfWork.Category.Add(category);
                await _unitOfWork.SaveAsync();

                TempData["Success"] = "Category created successfully!";
                return RedirectToAction(nameof(GetAllCategories));
            }

            return View(category);
        }

        // GET: Category/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _unitOfWork.Category.GetAsync(c => c.Id == id);
            if (category == null || category.IsDeleted)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _unitOfWork.Category.Update(category);
                    await _unitOfWork.SaveAsync();

                    TempData["Success"] = "Category updated successfully!";
                }
                catch (Exception)
                {
                    if (!await CategoryExistsAsync(category.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(GetAllCategories));
            }
            return View(category);
        }

        // GET: Category/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _unitOfWork.Category.GetAsync(c => c.Id == id && !c.IsDeleted);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Category/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _unitOfWork.Category.GetAsync(c => c.Id == id);
            if (category != null)
            {
                category.IsDeleted = true; // Soft delete
                _unitOfWork.Category.Update(category);
                await _unitOfWork.SaveAsync();

                TempData["Success"] = "Category deleted successfully!";
            }

            return Json(new { success = true });
        }

        // POST: Toggle Active Status
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var category = await _unitOfWork.Category.GetAsync(c => c.Id == id);
            if (category != null && !category.IsDeleted)
            {
                category.IsActive = !category.IsActive;
                _unitOfWork.Category.Update(category);
                await _unitOfWork.SaveAsync();

                TempData["Success"] = $"Category {(category.IsActive ? "activated" : "deactivated")} successfully!";
            }

            return RedirectToAction(nameof(GetAllCategories));
        }

        private async Task<bool> CategoryExistsAsync(int id)
        {
            var category = await _unitOfWork.Category.GetAsync(c => c.Id == id && !c.IsDeleted);
            return category != null;
        }
    }
}