using BookECommerce.Controllers;
using BookECommerce.Data;
using BookECommerce.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace BookECommerce.Tests.ControllersTests
{
    [TestFixture]
    public class CategoryControllerTests
    {
        private ApplicationDbContext _dbContext;
        private CategoryController _sut;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _dbContext = new ApplicationDbContext(options);
            ResetDatabase();
            SeedDatabase();
            _sut = new CategoryController(_dbContext);
            SetupTempData();
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
            _sut = null;
        }
        
        private void SeedDatabase()
        {
            _dbContext.Categories.AddRange(
                new Category { Id = 1, Name = "Action", DisplayOrder = 1 },
                new Category { Id = 2, Name = "SciFi", DisplayOrder = 2 },
                new Category { Id = 3, Name = "History", DisplayOrder = 3 }
            );
            _dbContext.SaveChanges();
        }
        
        private void SetupTempData()
        {
            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            _sut.TempData = tempData;
        }

        private void ResetDatabase()
        {
            _dbContext.Categories.RemoveRange(_dbContext.Categories);
            _dbContext.SaveChanges();
        }
        
        [Test]
        public void CategoryController_Index_ReturnsViewWithCategoryList()
        {
            // Arrange
            var expectedCategoryList = _dbContext.Categories.ToList();

            // Act
            var result = _sut.Index();

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = result as ViewResult;
            var model = viewResult?.Model as List<Category>;
            Assert.IsInstanceOf<List<Category>>(viewResult?.Model);
            Assert.That(model, Is.EqualTo(expectedCategoryList));
        }
        
        [Test]
        public void CategoryController_CreateNewCategory_Post_CategoryNameEqualsDisplayOrder_ReturnsViewWithModelError()
        {
            // Arrange
            var category = new Category {Name = "1", DisplayOrder = 1 };

            // Act
            var result = _sut.CreateNewCategory(category);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = result as ViewResult;
            var modelState = viewResult?.ViewData.ModelState;
            Assert.IsTrue(modelState?.ContainsKey("Name"));
            Assert.That(modelState?["Name"]?.Errors[0].ErrorMessage, Is.EqualTo("Category Name cannot be the same as Display Order"));
        }
        
        [Test]
        public void CategoryController_CreateNewCategory_Post_CategoryAlreadyExists_ReturnsViewWithModelError()
        {
            // Arrange
            var category = new Category {Name = "Action", DisplayOrder = 1 };

            // Act
            var result = _sut.CreateNewCategory(category);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = result as ViewResult;
            var modelState = viewResult?.ViewData.ModelState;
            Assert.IsTrue(modelState?.ContainsKey("Name"));
            Assert.That(modelState?["Name"]?.Errors[0].ErrorMessage, Is.EqualTo("Category with this name already exists"));
        }

        [Test]
        public void CategoryController_CreateNewCategory_Post_ValidCategory_ReturnsRedirectToIndex()
        {
            // Arrange
            var category = new Category {Name = "Fiction", DisplayOrder = 1 };

            // Act
            var result = _sut.CreateNewCategory(category);

            // Assert
            var addedCategory = _dbContext.Categories.FirstOrDefault(c => c.Id == category.Id);
            Assert.IsNotNull(addedCategory);
            Assert.That(addedCategory?.Name, Is.EqualTo(category.Name));
            Assert.That(addedCategory?.DisplayOrder, Is.EqualTo(category.DisplayOrder));
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.That(((RedirectToActionResult)result).ActionName, Is.EqualTo("Index"));
        }
        
        [Test]
        public void CategoryController_Edit_Post_CategoryNameEqualsDisplayOrder_ReturnsViewWithModelError()
        {
            // Arrange
            var category = new Category {Name = "22", DisplayOrder = 22 };

            // Act
            var result = _sut.Edit(category);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = result as ViewResult;
            var modelState = viewResult?.ViewData.ModelState;
            Assert.IsTrue(modelState?.ContainsKey("Name"));
            Assert.That(modelState?["Name"]?.Errors[0].ErrorMessage, Is.EqualTo("Category Name cannot be the same as Display Order"));
        }
        
        [Test]
        public void CategoryController_Edit_Post_CategoryAlreadyExists_ReturnsViewWithModelError()
        {
            // Arrange
            var category = new Category {Name = "SciFi", DisplayOrder = 22 };

            // Act
            var result = _sut.Edit(category);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = result as ViewResult;
            var modelState = viewResult?.ViewData.ModelState;
            Assert.IsTrue(modelState?.ContainsKey("Name"));
            Assert.That(modelState?["Name"]?.Errors[0].ErrorMessage, Is.EqualTo("Category with this name already exists"));
        }
        
        [Test]
        public void CategoryController_Edit_Post_ValidCategory_ReturnsRedirectToIndex()
        {
            // Arrange
            var category = new Category {Name = "Fiction", DisplayOrder = 1 };

            // Act
            var result = _sut.Edit(category);

            // Assert
            var editedCategory = _dbContext.Categories.FirstOrDefault(c => c.Id == category.Id);
            Assert.IsNotNull(editedCategory);
            Assert.That(editedCategory?.Name, Is.EqualTo(category.Name));
            Assert.That(editedCategory?.DisplayOrder, Is.EqualTo(category.DisplayOrder));
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.That(((RedirectToActionResult)result).ActionName, Is.EqualTo("Index"));
        }
        
        [Test]
        public void CategoryController_DeletePost_InvalidId_ReturnsNotFound()
        {
            // Arrange
            int invalidCategoryId = 999;

            // Act
            var result = _sut.DeletePost(invalidCategoryId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }
        
        [Test]
        public void CategoryController_DeletePost_ValidId_ReturnsRedirectToIndex()
        {
            // Arrange
            int validCategoryId = 1;

            // Act
            var result = _sut.DeletePost(validCategoryId);

            // Assert
            var deletedCategory = _dbContext.Categories.FirstOrDefault(c => c.Id == validCategoryId);
            Assert.IsNull(deletedCategory);
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.That(((RedirectToActionResult)result).ActionName, Is.EqualTo("Index"));
        }
    }
}