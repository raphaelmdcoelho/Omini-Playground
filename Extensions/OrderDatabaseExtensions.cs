using Microsoft.EntityFrameworkCore;

public static class OrderDatabaseExtensions
{
    public static IServiceCollection AddOrderDatabase(this IServiceCollection services)
    {
        services.AddDbContext<OrderDbContext>(dbContextOptionsBuilder => {
            dbContextOptionsBuilder.UseInMemoryDatabase("OrdersDb");
        });

        return services;
    }
}