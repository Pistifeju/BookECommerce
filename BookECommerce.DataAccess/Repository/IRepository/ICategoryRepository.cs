using System.Linq.Expressions;
using BookECommerce.Models;

namespace BookECommerce.DataAccess.Repository.IRepository;

public interface ICategoryRepository : IRepository<Category>
{
    void Update(Category category);
}