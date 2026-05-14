using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ExampleOrderService.Data;
using ExampleOrderService.Models;

namespace ExampleOrderService.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context) =>
            _context = context;

        public async Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken = default) =>
            await _context.Orders
                .AsNoTracking()
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync(cancellationToken);

        public async Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
            await _context.Orders
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

        public async Task<Order> CreateAsync(Order order, CancellationToken cancellationToken = default)
        {
            order.Id = 0;
            order.RowVersion = null;
            if (order.OrderDate == default)
                order.OrderDate = System.DateTime.UtcNow;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync(cancellationToken);
            return order;
        }

        public async Task<UpdateResult> UpdateAsync(Order order, byte[]? originalRowVersion = null, CancellationToken cancellationToken = default)
        {
            var existing = await _context.Orders.FirstOrDefaultAsync(o => o.Id == order.Id, cancellationToken);
            if (existing is null)
                return UpdateResult.NotFound;

            // Set original RowVersion for optimistic concurrency, if supplied by the caller
            if (originalRowVersion is not null)
                _context.Entry(existing).Property(e => e.RowVersion).OriginalValue = originalRowVersion;

            // Map updatable fields
            existing.ProductName = order.ProductName;
            existing.Quantity = order.Quantity;
            existing.Price = order.Price;
            existing.OrderDate = order.OrderDate;

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
                return UpdateResult.Success;
            }
            catch (DbUpdateConcurrencyException)
            {
                // If deleted concurrently
                if (!await _context.Orders.AnyAsync(e => e.Id == order.Id, cancellationToken))
                    return UpdateResult.NotFound;

                return UpdateResult.Conflict;
            }
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var existing = await _context.Orders.FindAsync(new object[] { id }, cancellationToken);
            if (existing is null)
                return false;

            _context.Orders.Remove(existing);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default) =>
            _context.Orders.AnyAsync(e => e.Id == id, cancellationToken);
    }
}