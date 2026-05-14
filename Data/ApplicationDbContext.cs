using Microsoft.EntityFrameworkCore;
using ExampleOrderService.Models;

namespace ExampleOrderService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.ProductName)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(e => e.Quantity)
                      .IsRequired();

                entity.Property(e => e.Price)
                      .IsRequired()
                      .HasColumnType("decimal(18,2)");

                entity.Property(e => e.OrderDate)
                      .IsRequired();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}