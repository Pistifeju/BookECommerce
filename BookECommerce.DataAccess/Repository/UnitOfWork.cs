using BookECommerce.DataAccess.Data;
using BookECommerce.DataAccess.Repository.IRepository;

namespace BookECommerce.DataAccess.Repository;

public class UnitOfWork : IUnitOfWork
{
    public ICategoryRepository CategoryRepository { get; private set; }
    public IProductRepository ProductRepository { get; private set; }
    
    public ICompanyRepository CompanyRepository { get; private set; }
    
    private ApplicationDbContext _dbContext;

    public UnitOfWork(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        CategoryRepository = new CategoryRepository(_dbContext);
        ProductRepository = new ProductRepository(_dbContext);
        CompanyRepository = new CompanyRepository(_dbContext);
    }

    public void Save()
    {
        _dbContext.SaveChanges();
    }
}