using System.Linq.Expressions;
using BookECommerce.Models;

namespace BookECommerce.DataAccess.Repository.IRepository;

public interface ICompanyRepository : IRepository<Company>
{
    void Update(Company category);
}