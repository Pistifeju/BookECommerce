using BookECommerce.DataAccess.Data;
using BookECommerce.DataAccess.Repository.IRepository;
using BookECommerce.Models;

namespace BookECommerce.DataAccess.Repository;

public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
{
    private ApplicationDbContext _dbContext;
    
    public OrderHeaderRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public void Update(OrderHeader orderHeader)
    {
        _dbContext.OrderHeaders.Update(orderHeader);
    }
}