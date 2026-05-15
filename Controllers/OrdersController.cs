using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ExampleOrderService.Models;
using ExampleOrderService.Repositories;

namespace ExampleOrderService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _repository;

        public OrdersController(IOrderRepository repository) =>
            _repository = repository;

        // GET: api/orders  
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders(CancellationToken cancellationToken = default)
        {
            var orders = await _repository.GetAllAsync(cancellationToken);
            return Ok(orders);
        }

        // GET: api/orders/{id}
        [HttpGet("{id:int}", Name = nameof(GetOrder))]
        public async Task<ActionResult<Order>> GetOrder(int id, CancellationToken cancellationToken = default)
        {
            var order = await _repository.GetByIdAsync(id, cancellationToken);

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

            var created = await _repository.CreateAsync(order, cancellationToken);
            return CreatedAtAction(nameof(GetOrder), new { id = created.Id }, created);
        }

        // PUT: api/orders/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] Order updatedOrder, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != updatedOrder.Id)
                return BadRequest("Route id and body id do not match.");

            var result = await _repository.UpdateAsync(updatedOrder, updatedOrder.RowVersion, cancellationToken);
            return result switch
            {
                UpdateResult.Success => NoContent(),
                UpdateResult.NotFound => NotFound(),
                UpdateResult.Conflict => (IActionResult)Conflict(new
                {
                    Message = "Concurrency conflict: the order was modified by another actor.",
                    Current = await _repository.GetByIdAsync(id, cancellationToken)
                }),
                _ => StatusCode(500)
            };
        }

        // DELETE: api/orders/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteOrder(int id, CancellationToken cancellationToken = default)
        {
            var deleted = await _repository.DeleteAsync(id, cancellationToken);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}