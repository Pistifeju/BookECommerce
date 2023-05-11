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

    public void Update(Category category)
    {
        _dbContext.Categories.Update(category);
    }
}