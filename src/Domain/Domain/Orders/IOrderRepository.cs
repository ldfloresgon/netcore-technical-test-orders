using Core.Domain.Orders.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Domain.Orders
{
    public interface IOrderRepository
    {
        Task<bool> Insert(Order order, CancellationToken cancellationToken);
    }
}
