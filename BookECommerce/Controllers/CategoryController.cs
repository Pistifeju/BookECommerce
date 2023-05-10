using BookECommerce.DataAccess.Data;
using BookECommerce.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookECommerce.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        
        public CategoryController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public IActionResult Index()
        {
            var categoryList = _dbContext.Categories.ToList();
            return View(categoryList);
        }

        public IActionResult CreateNewCategory() => View();
        
        [HttpPost]
        public IActionResult CreateNewCategory(Category category)
        {
            if (CategoryNameIsEqualToDisplayOrder(category))
            {
                ModelState.AddModelError("Name", "Category Name cannot be the same as Display Order");
            }
            
            if (CategoryAlreadyExists(category))
            {
                ModelState.AddModelError("Name", "Category with this name already exists");
            }
            
            if (ModelState.IsValid)
            {
                _dbContext.Categories.Add(category);
                _dbContext.SaveChanges();
                TempData["Success"] = "Category created successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }
        
        public IActionResult Edit(int? id) => FindCategoryViewById(id);
        
        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (CategoryNameIsEqualToDisplayOrder(category))
            {
                ModelState.AddModelError("Name", "Category Name cannot be the same as Display Order");
            }

            if (CategoryAlreadyExists(category))
            {
                ModelState.AddModelError("Name", "Category with this name already exists");
            }
            
            if (ModelState.IsValid)
            {
                _dbContext.Categories.Update(category);
                _dbContext.SaveChanges();
                TempData["Success"] = "Category updated successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }
        
        public IActionResult Delete(int? id) => FindCategoryViewById(id);
        
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            var category = _dbContext.Categories.Find(id);
            
            if (category == null) return NotFound();
            
            _dbContext.Categories.Remove(category);
            _dbContext.SaveChanges();
            TempData["Success"] = "Category deleted successfully";
            return RedirectToAction(nameof(Index));
        }

        private IActionResult FindCategoryViewById(int? id)
        {
            if (id is null or 0) return NotFound();
            
            var categoryFromDb = _dbContext.Categories.Find(id);
            if (categoryFromDb == null) return NotFound();
            
            return View(categoryFromDb);
        }

        private static bool CategoryNameIsEqualToDisplayOrder(Category category)
        {
            return category.Name == category.DisplayOrder.ToString();
        }

        private bool CategoryAlreadyExists(Category category)
        {
            var doesCategoryNameAlreadyExist = _dbContext.Categories
                .Any(c => c.Name == category.Name && c.Id != category.Id);
            return doesCategoryNameAlreadyExist;
        }
    }
}
