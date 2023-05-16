using BookECommerce.DataAccess.Repository.IRepository;
using BookECommerce.Models;
using BookECommerce.Models.ViewModels;
using BookECommerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookECommerce.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]
public class CompanyController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    
    public CompanyController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        var companyList = _unitOfWork.CompanyRepository.GetAll().ToList();
        return View(companyList);
    }

    public IActionResult Upsert(int? id)
    {
        if (id is null or 0) return View(new Company());
        
        Company company = _unitOfWork.CompanyRepository.Get(c => c.Id == id);
        return View(company);
    }

    [HttpPost]
    public IActionResult Upsert(Company company)
    {
        if (!ModelState.IsValid)
        {
            return View(company);
        }
        
        if (company.Id == 0)
        {
            _unitOfWork.CompanyRepository.Add(company);
        }
        else
        {
            _unitOfWork.CompanyRepository.Update(company);
        }
        
        _unitOfWork.Save();
        TempData["Success"] = "Company created successfully";
        return RedirectToAction(nameof(Index));
    }

    #region API CALLS
    
    [HttpGet]
    public IActionResult GetAll()
    {
        var allObj = _unitOfWork.CompanyRepository.GetAll();
        return Json(new {data = allObj});
    }
    
    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        var objFromDb = _unitOfWork.CompanyRepository.Get(c => c.Id == id);
        
        if (objFromDb == null) return Json(new {success = false, message = "Error while deleting"});
        
        _unitOfWork.CompanyRepository.Remove(objFromDb);
        _unitOfWork.Save();
        
        return Json(new {success = true, message = "Delete successful"});
    }

    #endregion
}