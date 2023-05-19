using System.Linq.Expressions;
using BookECommerce.Models;

namespace BookECommerce.DataAccess.Repository.IRepository;

public interface IOrderHeaderRepository : IRepository<OrderHeader>
{
    void Update(OrderHeader orderHeader);
    void UpdateStatus(int id, string status, string? paymentStatus = null);
    
    void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId);
}