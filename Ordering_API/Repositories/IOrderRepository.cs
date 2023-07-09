using Ordering_API.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ordering_API.Repositories
{
    public interface IOrderRepository : IAsyncRepository<Order>
    {
        Task<IEnumerable<Order>> GetOrdersByUserName(string userName);
    }
}
