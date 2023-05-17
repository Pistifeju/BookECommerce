using System.Linq.Expressions;
using BookECommerce.Models;

namespace BookECommerce.DataAccess.Repository.IRepository;

public interface IOrderDetailRepository : IRepository<OrderDetail>
{
    void Update(OrderDetail orderDetail);
}