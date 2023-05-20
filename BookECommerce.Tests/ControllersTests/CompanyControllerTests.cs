using BookECommerce.Areas.Admin.Controllers;
using BookECommerce.DataAccess.Data;
using BookECommerce.DataAccess.Repository;
using BookECommerce.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Moq;
using NUnit.Framework;

namespace BookECommerce.Tests.ControllersTests;

[TestFixture]
public class CompanyControllerTests
{
    private ApplicationDbContext _dbContext;
    private CompanyController _sut;
    
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        _dbContext = new ApplicationDbContext(options);
        _sut = new CompanyController(new UnitOfWork(_dbContext));
        ResetDatabase();
        SetupTempData();
    }
    
    [TearDown]
    public void TearDown()
    {
        ResetDatabase();
        _dbContext.Dispose();
        _sut.Dispose();
    }
    
    private void SetupTempData()
    {
        var httpContext = new DefaultHttpContext();
        var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        _sut.TempData = tempData;
    }
    
    private void ResetDatabase()
    {
        _dbContext.Companies.RemoveRange(_dbContext.Companies);
        _dbContext.SaveChanges();
    }
    
    [Test]
    public void CompanyController_Index_ReturnsViewWithCompanyList()
    {
        // Arrange
        var companies = new List<Company> { new Company {Name = "ABC"}, new Company {Name = "CBA"} };
        _dbContext.Companies.AddRange(companies);
        _dbContext.SaveChanges();
    
        // Act
        var result = _sut.Index();
    
        // Assert
        var viewResult = result as ViewResult;
        var model = viewResult.Model as List<Company>;
        Assert.That(result, Is.TypeOf<ViewResult>());
        Assert.That(model, Is.EquivalentTo(companies));
    }
    
    [Test]
    public void CompanyController_Upsert_WithoutIdReturnsNewCompany()
    {
        // Arrange
        int? id = null;

        // Act
        var result = _sut.Upsert(id);

        // Assert
        var viewResult = result as ViewResult;
        var model = viewResult.Model as Company;
        Assert.That(result, Is.TypeOf<ViewResult>());
        Assert.IsNotNull(model);
        Assert.That(model.Id, Is.EqualTo(0));
    }

    [Test]
    public void CompanyController_Upsert_WithIdReturnCompanyFromRepository()
    {
        // Arrange
        var company = new Company { Id = 1, Name = "ABC" };
        _dbContext.Companies.Add(company);
        _dbContext.SaveChanges();
        
        // Act
        var result = _sut.Upsert(company.Id);
        
        // Assert
        var viewResult = result as ViewResult;
        var model = viewResult.Model as Company;
        Assert.That(result, Is.TypeOf<ViewResult>());
        Assert.IsNotNull(model);
        Assert.That(model.Id, Is.EqualTo(company.Id));
        Assert.That(model.Name, Is.EqualTo(company.Name));
    }
    
    [Test]
    public void CompanyController_UpsertPost_InvalidModelStateReturnsViewWithCompany()
    {
        // Arrange
        var company = new Company { Id = 1, Name = "ABC" };
        _sut.ModelState.AddModelError("Name", "Name is required");
        
        // Act
        var result = _sut.Upsert(company);
        
        // Assert
        var viewResult = result as ViewResult;
        Assert.That(result, Is.TypeOf<ViewResult>());
        Assert.That(viewResult.Model, Is.EqualTo(company));
    }
    
    [Test]
    public void CompanyController_UpsertPost_NewCompanyReturnsRedirectToAction()
    {
        // Arrange
        var company = new Company { Id = 0, Name = "ABC" };
        
        // Act
        var result = _sut.Upsert(company);
        
        // Assert
        var redirectToActionResult = result as RedirectToActionResult;
        Assert.That(result, Is.TypeOf<RedirectToActionResult>());
        Assert.That(redirectToActionResult.ActionName, Is.EqualTo("Index"));
        Assert.That(1, Is.EqualTo(_dbContext.Companies.Count()));
        Assert.That(_sut.TempData["Success"], Is.EqualTo("Company created successfully"));
    }
    
    [Test]
    public void CompanyController_UpsertPost_ExistingCompanyReturnsRedirectToAction()
    {
        // Arrange
        var company = new Company { Id = 1, Name = "ABC" };
        _dbContext.Companies.Add(company);
        _dbContext.SaveChanges();
        
        // Act
        company.Name = "CBA";
        var result = _sut.Upsert(company);
        
        // Assert
        Assert.That(result, Is.TypeOf<RedirectToActionResult>());
        var updatedCompany = _dbContext.Companies.FirstOrDefault(c => c.Id == company.Id);
        Assert.That(1, Is.EqualTo(_dbContext.Companies.Count()));
        Assert.That(updatedCompany.Name, Is.EqualTo("CBA"));
        Assert.That(_sut.TempData["Success"], Is.EqualTo("Company updated successfully"));
    }
    
    [Test]
    public void CompanyController_GetAll_ReturnsJson()
    {
        // Arrange
        var company = new Company { Id = 1, Name = "ABC" };
        var company2 = new Company { Id = 2, Name = "ABC2" };
        _dbContext.Companies.Add(company);
        _dbContext.Companies.Add(company2);
        _dbContext.SaveChanges();
        
        // Act
        var result = _sut.GetAll();
    
        // Assert
        Assert.That(result, Is.TypeOf<JsonResult>());
    }

    [Test]
    public void CompanyController_Delete_ReturnsJson()
    {
        // Arrange
        var company = new Company { Id = 1, Name = "ABC" };
        _dbContext.Companies.Add(company);
        _dbContext.SaveChanges();
        
        // Act
        var result = _sut.Delete(1);
        
        // Assert
        Assert.That(result, Is.TypeOf<JsonResult>());
        Assert.That(_dbContext.Companies.Count(), Is.EqualTo(0));
    }
}