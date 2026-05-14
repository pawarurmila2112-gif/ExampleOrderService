using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using ExampleOrderService.Data;
using ExampleOrderService.Models;

namespace ExampleOrderService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context) =>
            _context = context;

        // GET: api/orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders(CancellationToken cancellationToken = default)
        {
            var orders = await _context.Orders
                .AsNoTracking()
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync(cancellationToken);

            return Ok(orders);
        }

        // GET: api/orders/{id}
        [HttpGet("{id:int}", Name = nameof(GetOrder))]
        public async Task<ActionResult<Order>> GetOrder(int id, CancellationToken cancellationToken = default)
        {
            var order = await _context.Orders
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

            if (order is null)
                return NotFound();

            return Ok(order);
        }

        // POST: api/orders
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder([FromBody] Order order, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Ignore client-supplied Id/RowVersion for creation
            order.Id = 0;
            order.RowVersion = null;

            if (order.OrderDate == default)
                order.OrderDate = DateTime.UtcNow;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync(cancellationToken);

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }

        // PUT: api/orders/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] Order updatedOrder, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != updatedOrder.Id)
                return BadRequest("Route id and body id do not match.");

            var existing = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
            if (existing is null)
                return NotFound();

            // Prepare concurrency: set the original RowVersion to the value supplied by the client
            if (updatedOrder.RowVersion is not null)
            {
                _context.Entry(existing).Property(e => e.RowVersion).OriginalValue = updatedOrder.RowVersion;
            }

            // Map allowed updatable fields
            existing.ProductName = updatedOrder.ProductName;
            existing.Quantity = updatedOrder.Quantity;
            existing.Price = updatedOrder.Price;
            existing.OrderDate = updatedOrder.OrderDate;

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                // If the entity was deleted concurrently, return 404.
                if (!await _context.Orders.AnyAsync(e => e.Id == id, cancellationToken))
                    return NotFound();

                // Otherwise it's a concurrency conflict — return 409 Conflict with current store values
                var storeValues = await _context.Orders
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

                return Conflict(new
                {
                    Message = "Concurrency conflict: the order was modified by another actor.",
                    Current = storeValues
                });
            }
        }

        // DELETE: api/orders/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteOrder(int id, CancellationToken cancellationToken = default)
        {
            var existing = await _context.Orders.FindAsync(new object[] { id }, cancellationToken);
            if (existing is null)
                return NotFound();

            _context.Orders.Remove(existing);
            await _context.SaveChangesAsync(cancellationToken);

            return NoContent();
        }
    }
}