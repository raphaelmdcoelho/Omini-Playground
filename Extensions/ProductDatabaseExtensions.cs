using Microsoft.EntityFrameworkCore;

public static class ProductDatabaseExtensions
{
    public static IServiceCollection AddProductDatabase(this IServiceCollection services)
    {
        services.AddDbContext<ProductDbContext>(dbContextOptionsBuilder => {
            dbContextOptionsBuilder.UseSqlite("Data Source=products.db");
        });

        return services;
    }
}