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

    public ProductController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        var productList = _unitOfWork.ProductRepository.GetAll().ToList();
        return View(productList);
    }

    public IActionResult CreateNewProduct()
    {
        IEnumerable<SelectListItem> categoryList = _unitOfWork.CategoryRepository.GetAll()
            .Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
        
        ProductViewModel productViewModel = new()
        {
            Product = new Product(),
            CategoryList = categoryList
        };
        return View(productViewModel);
    }

    [HttpPost]
    public IActionResult CreateNewProduct(ProductViewModel viewModel)
    {
        // TODO: Check if product already exists with this Id

        if (ModelState.IsValid)
        {
            _unitOfWork.ProductRepository.Add(viewModel.Product);
            _unitOfWork.Save();
            TempData["Success"] = "Product created successfully";
            return RedirectToAction(nameof(Index));
        }

        return View();
    }

    public IActionResult Edit(int? id) => FindProductViewById(id);

    [HttpPost]
    public IActionResult Edit(Product product)
    {
        if (ModelState.IsValid)
        {
            _unitOfWork.ProductRepository.Update(product);
            _unitOfWork.Save();
            TempData["Success"] = "Product updated successfully";
            return RedirectToAction(nameof(Index));
        }

        return View(product);
    }

    public IActionResult Delete(int? id) => FindProductViewById(id);

    [HttpPost, ActionName("Delete")]
    public IActionResult DeletePost(int? id)
    {
        var product = _unitOfWork.ProductRepository.Get(c => c.Id == id);

        if (product == null) return NotFound();

        _unitOfWork.ProductRepository.Remove(product);
        _unitOfWork.Save();
        TempData["Success"] = "Product deleted successfully";

        return RedirectToAction(nameof(Index));
    }

    private IActionResult FindProductViewById(int? id)
    {
        if (id is null or 0) return NotFound();

        var productFromDb = _unitOfWork.ProductRepository.Get(c => c.Id == id);
        if (productFromDb == null) return NotFound();

        return View(productFromDb);
    }
}