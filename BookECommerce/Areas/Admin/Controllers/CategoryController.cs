using BookECommerce.DataAccess.Repository.IRepository;
using BookECommerce.Models;
using BookECommerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookECommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        public IActionResult Index()
        {
            var categoryList = _unitOfWork.CategoryRepository.GetAll().ToList();
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
            
            if (_unitOfWork.CategoryRepository.CategoryAlreadyExists(category))
            {
                ModelState.AddModelError("Name", "Category with this name already exists");
            }
            
            if (ModelState.IsValid)
            {
                _unitOfWork.CategoryRepository.Add(category);
                _unitOfWork.Save();
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

            if (_unitOfWork.CategoryRepository.CategoryAlreadyExists(category))
            {
                ModelState.AddModelError("Name", "Category with this name already exists");
            }

            if (ModelState.IsValid)
            {
                _unitOfWork.CategoryRepository.Update(category);
                _unitOfWork.Save();
                TempData["Success"] = "Category updated successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }
        
        public IActionResult Delete(int? id) => FindCategoryViewById(id);
        
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            var category = _unitOfWork.CategoryRepository.Get(c => c.Id == id);            
            
            if (category == null) return NotFound();
            
            _unitOfWork.CategoryRepository.Remove(category);
            _unitOfWork.Save();
            
            TempData["Success"] = "Category deleted successfully";
            
            return RedirectToAction(nameof(Index));
        }

        private IActionResult FindCategoryViewById(int? id)
        {
            if (id is null or 0) return NotFound();
            
            var categoryFromDb = _unitOfWork.CategoryRepository.Get(c => c.Id == id);
            if (categoryFromDb == null) return NotFound();
            
            return View(categoryFromDb);
        }

        private static bool CategoryNameIsEqualToDisplayOrder(Category category)
        {
            return category.Name == category.DisplayOrder.ToString();
        }
    }
}
