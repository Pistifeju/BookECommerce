using System.Diagnostics;
using System.Security.Claims;
using BookECommerce.DataAccess.Repository.IRepository;
using BookECommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookECommerce.Areas.Customer.Controllers;

[Area("Customer")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUnitOfWork _unitOfWork;
    public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }
    
    public IActionResult Index()
    {
        IEnumerable<Product> productList = _unitOfWork.ProductRepository.GetAll(includeProperties:"Category");
        return View(productList);
    }
    
    public IActionResult Details(int id)
    {
        Product product = _unitOfWork.ProductRepository.Get(p=> p.Id == id, includeProperties:"Category");
        ShoppingCart shoppingCart = new ShoppingCart()
        {
            Product = product,
            Count = 1,
            ProductId = id
        };
        return View(shoppingCart);
    } 
  
    [HttpPost]
    [Authorize]
    public IActionResult Details(ShoppingCart shoppingCart)
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return RedirectToAction(nameof(Index));
        }

        shoppingCart.ApplicationUserId = userId;

        ShoppingCart cartFromDatabase = _unitOfWork.ShoppingCartRepository
            .Get(u => u.ApplicationUserId == userId && u.ProductId == shoppingCart.ProductId);

        if (cartFromDatabase != null)
        {
            cartFromDatabase.Count += shoppingCart.Count;
            _unitOfWork.ShoppingCartRepository.Update(cartFromDatabase);
        }
        else
        {
            _unitOfWork.ShoppingCartRepository.Add(shoppingCart);
        }

        shoppingCart.Id = 0; //Hackish way to solve the IDENTITY_INSERT to ON error
        _unitOfWork.Save();

        return RedirectToAction(nameof(Index));
    }


    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}