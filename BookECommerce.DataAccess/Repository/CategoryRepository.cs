using System.Linq.Expressions;
using BookECommerce.DataAccess.Data;
using BookECommerce.DataAccess.Repository.IRepository;
using BookECommerce.Models;

namespace BookECommerce.DataAccess.Repository;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    private ApplicationDbContext _dbContext;

    public CategoryRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public bool CategoryAlreadyExists(Category category)
    {
        var existingCategory = Get(c => c.Name == category.Name && c.Id != category.Id);
    
        if (existingCategory != null)
        {
            DetachEntity(category);
            return true;
        }

        return false;
    }
}