using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExampleOrderService.Models;

namespace ExampleOrderService.Repositories
{
    public enum UpdateResult
    {
        Success,
        NotFound,
        Conflict
    }

    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Order> CreateAsync(Order order, CancellationToken cancellationToken = default);
        Task<UpdateResult> UpdateAsync(Order order, byte[]? originalRowVersion = null, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    }
}