using BookECommerce.DataAccess.Data;
using BookECommerce.DataAccess.Repository.IRepository;
using BookECommerce.Models;

namespace BookECommerce.DataAccess.Repository;

public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
{
    private ApplicationDbContext _dbContext;
    
    public OrderDetailRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public void Update(OrderDetail orderDetail)
    {
        _dbContext.OrderDetails.Update(orderDetail);
    }
}