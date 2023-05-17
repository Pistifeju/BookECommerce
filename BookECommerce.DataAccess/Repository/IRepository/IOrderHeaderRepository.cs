using System.Linq.Expressions;
using BookECommerce.Models;

namespace BookECommerce.DataAccess.Repository.IRepository;

public interface IOrderHeaderRepository : IRepository<OrderHeader>
{
    void Update(OrderHeader orderHeader);
}