using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;
using ExampleOrderService.Controllers;
using ExampleOrderService.Data;
using ExampleOrderService.Models;

namespace ExampleOrderService.Tests
{
    public class OrdersControllerTests
    {
        private static ApplicationDbContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task GetOrders_ReturnsOkWithOrders()
        {
            var dbName = Guid.NewGuid().ToString();
            await using var context = CreateContext(dbName);
            context.Orders.AddRange(new[]
            {
                new Order { ProductName = "A", Quantity = 1, Price = 10m, OrderDate = DateTime.UtcNow },
                new Order { ProductName = "B", Quantity = 2, Price = 20m, OrderDate = DateTime.UtcNow }
            });
            await context.SaveChangesAsync();

            var controller = new OrdersController(context);

            var actionResult = await controller.GetOrders();
            var ok = Assert.IsType<OkObjectResult>(actionResult.Result);
            var orders = Assert.IsAssignableFrom<IEnumerable<Order>>(ok.Value);
            Assert.Equal(2, orders.Count());
        }

        [Fact]
        public async Task GetOrder_ReturnsNotFound_WhenMissing()
        {
            var dbName = Guid.NewGuid().ToString();
            await using var context = CreateContext(dbName);
            var controller = new OrdersController(context);

            var result = await controller.GetOrder(12345);
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateOrder_CreatesAndReturnsCreated()
        {
            var dbName = Guid.NewGuid().ToString();
            await using var context = CreateContext(dbName);
            var controller = new OrdersController(context);

            var order = new Order
            {
                ProductName = "New",
                Quantity = 3,
                Price = 30m,
                OrderDate = DateTime.UtcNow
            };

            var actionResult = await controller.CreateOrder(order);
            var created = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var createdOrder = Assert.IsType<Order>(created.Value);
            Assert.True(createdOrder.Id > 0);

            // verify saved
            var saved = await context.Orders.FindAsync(createdOrder.Id);
            Assert.NotNull(saved);
            Assert.Equal("New", saved!.ProductName);
        }

        [Fact]
        public async Task UpdateOrder_ReturnsNoContent_WhenSuccessful()
        {
            var dbName = Guid.NewGuid().ToString();
            await using var context = CreateContext(dbName);
            var initial = new Order
            {
                ProductName = "Before",
                Quantity = 1,
                Price = 5m,
                OrderDate = DateTime.UtcNow
            };
            context.Orders.Add(initial);
            await context.SaveChangesAsync();

            var controller = new OrdersController(context);

            var updated = new Order
            {
                Id = initial.Id,
                ProductName = "After",
                Quantity = 10,
                Price = 50m,
                OrderDate = initial.OrderDate
            };

            var result = await controller.UpdateOrder(initial.Id, updated);
            Assert.IsType<NoContentResult>(result);

            var reloaded = await context.Orders.FindAsync(initial.Id);
            Assert.Equal("After", reloaded!.ProductName);
            Assert.Equal(10, reloaded.Quantity);
        }

        [Fact]
        public async Task DeleteOrder_ReturnsNoContent_WhenSuccessful()
        {
            var dbName = Guid.NewGuid().ToString();
            await using var context = CreateContext(dbName);
            var order = new Order
            {
                ProductName = "ToDelete",
                Quantity = 1,
                Price = 1m,
                OrderDate = DateTime.UtcNow
            };
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            var controller = new OrdersController(context);

            var result = await controller.DeleteOrder(order.Id);
            Assert.IsType<NoContentResult>(result);

            var found = await context.Orders.FindAsync(order.Id);
            Assert.Null(found);
        }
    }
}