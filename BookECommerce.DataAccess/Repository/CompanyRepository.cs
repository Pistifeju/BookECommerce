using BookECommerce.DataAccess.Data;
using BookECommerce.DataAccess.Repository.IRepository;
using BookECommerce.Models;

namespace BookECommerce.DataAccess.Repository;

public class CompanyRepository : Repository<Company>, ICompanyRepository
{
    private ApplicationDbContext _dbContext;

    public CompanyRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
    
    public void Update(Company company)
    {
        _dbContext.Update(company);
    }
}