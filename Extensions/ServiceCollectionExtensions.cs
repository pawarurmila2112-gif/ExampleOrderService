using Microsoft.Extensions.DependencyInjection;
using ExampleOrderService.Repositories;

namespace ExampleOrderService.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOrderServices(this IServiceCollection services)
        {
            services.AddScoped<IOrderRepository, OrderRepository>();
            return services;
        }
    }
}