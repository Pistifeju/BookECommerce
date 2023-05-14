using BookECommerce.DataAccess.Repository.IRepository;
using BookECommerce.Models;
using BookECommerce.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookECommerce.Areas.Admin.Controllers;

[Area("Admin")]
public class ProductController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _webHostEnvironment;
    public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _webHostEnvironment = webHostEnvironment;
    }

    public IActionResult Index()
    {
        var productList = _unitOfWork.ProductRepository.GetAll(includeProperties:"Category").ToList();
        return View(productList);
    }

    public IActionResult Upsert(int? id)
    {
        var productViewModel = new ProductViewModel
        {
            Product = new Product(),
            CategoryList = GetCategoryList()
        };
        
        if (id is null or 0) return View(productViewModel);
        
        productViewModel.Product = _unitOfWork.ProductRepository.Get(c => c.Id == id);
        return View(productViewModel);
    }

    [HttpPost]
    public IActionResult Upsert(ProductViewModel viewModel, IFormFile? imageFile)
    {
        if (!ModelState.IsValid)
        {
            viewModel.CategoryList = GetCategoryList();
            return View(viewModel);
        }

        string wwwRootPath = _webHostEnvironment.WebRootPath;
        if (imageFile != null)
        {
            string fileName = Guid.NewGuid().ToString();
            var uploads = Path.Combine(wwwRootPath, @"images/product");
            var extension = Path.GetExtension(imageFile.FileName);

            if (!string.IsNullOrEmpty(viewModel.Product.ImageUrl))
            {
                string oldImagePath = Path.Combine(wwwRootPath, viewModel.Product.ImageUrl.TrimStart('@'));

                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }
            
            using (var fileSteam = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
            {
                imageFile.CopyTo(fileSteam);
            }
            
            viewModel.Product.ImageUrl = @"images/product/" + fileName + extension;
        }

        if (viewModel.Product.Id == 0)
        {
            _unitOfWork.ProductRepository.Add(viewModel.Product);
        }
        else
        {
            _unitOfWork.ProductRepository.Update(viewModel.Product);
        }
        
        _unitOfWork.Save();
        TempData["Success"] = "Product created successfully";
        return RedirectToAction(nameof(Index));
    }

    private IEnumerable<SelectListItem> GetCategoryList()
    {
        return _unitOfWork.CategoryRepository.GetAll()
            .Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
    }
    
    #region API CALLS
    
    [HttpGet]
    public IActionResult GetAll()
    {
        var allObj = _unitOfWork.ProductRepository.GetAll(includeProperties:"Category");
        return Json(new {data = allObj});
    }
    
    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        var objFromDb = _unitOfWork.ProductRepository.Get(c => c.Id == id);
        if (objFromDb == null) return Json(new {success = false, message = "Error while deleting"});
        
        string oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, objFromDb.ImageUrl.TrimStart('@'));

        if (System.IO.File.Exists(oldImagePath))
        {
            System.IO.File.Delete(oldImagePath);
        }
        
        _unitOfWork.ProductRepository.Remove(objFromDb);
        _unitOfWork.Save();
        
        return Json(new {success = true, message = "Delete successful"});
    }

    #endregion
}