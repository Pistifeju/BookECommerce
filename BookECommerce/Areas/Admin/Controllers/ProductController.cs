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
        var productViewModel = new ProductViewModel
        {
            Product = new Product(),
            CategoryList = GetCategoryList()
        };

        return View(productViewModel);
    }

    [HttpPost]
    public IActionResult CreateNewProduct(ProductViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            viewModel.CategoryList = GetCategoryList();
            return View(viewModel);
        }

        _unitOfWork.ProductRepository.Add(viewModel.Product);
        _unitOfWork.Save();
        TempData["Success"] = "Product created successfully";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(int? id) => FindProductViewById(id);

    [HttpPost]
    public IActionResult Edit(Product product)
    {
        if (!ModelState.IsValid) return View(product);

        _unitOfWork.ProductRepository.Update(product);
        _unitOfWork.Save();
        TempData["Success"] = "Product updated successfully";
        return RedirectToAction(nameof(Index));
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

    private IEnumerable<SelectListItem> GetCategoryList()
    {
        return _unitOfWork.CategoryRepository.GetAll()
            .Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
    }
}