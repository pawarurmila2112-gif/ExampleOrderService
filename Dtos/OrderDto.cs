using System;
using System.ComponentModel.DataAnnotations;
using ExampleOrderService.Models;

namespace ExampleOrderService.Dtos
{
    public sealed class OrderDto
    {
        public int Id { get; init; }

        public string ProductName { get; init; } = string.Empty;

        public int Quantity { get; init; }

        public decimal Price { get; init; }

        public DateTime OrderDate { get; init; }

        /// <summary>
        /// Base64-encoded concurrency token (RowVersion). Null when not present.
        /// </summary>
        public string? RowVersion { get; init; }

        public static OrderDto FromModel(Order order) =>
            new()
            {
                Id = order.Id,
                ProductName = order.ProductName,
                Quantity = order.Quantity,
                Price = order.Price,
                OrderDate = order.OrderDate,
                RowVersion = order.RowVersion is null ? null : Convert.ToBase64String(order.RowVersion)
            };
    }

    public sealed class OrderCreateDto
    {
        [Required]
        [StringLength(200)]
        public string ProductName { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; } = 1;

        [Range(0.0, double.MaxValue)]
        public decimal Price { get; set; } = 0m;

        public DateTime? OrderDate { get; set; }

        public Order ToModel() =>
            new()
            {
                ProductName = ProductName,
                Quantity = Quantity,
                Price = Price,
                OrderDate = OrderDate ?? DateTime.UtcNow
            };
    }

    public sealed class OrderUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string ProductName { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; } = 1;

        [Range(0.0, double.MaxValue)]
        public decimal Price { get; set; } = 0m;

        public DateTime OrderDate { get; set; }

        /// <summary>
        /// Base64-encoded concurrency token (RowVersion) supplied by the client.
        /// </summary>
        [Required]
        public string RowVersion { get; set; } = string.Empty;

        public byte[]? GetRowVersionBytes() =>
            string.IsNullOrEmpty(RowVersion) ? null : Convert.FromBase64String(RowVersion);

        public void ApplyTo(Order order)
        {
            order.ProductName = ProductName;
            order.Quantity = Quantity;
            order.Price = Price;
            order.OrderDate = OrderDate;
        }
    }
}