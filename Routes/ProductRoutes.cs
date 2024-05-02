using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public static class ProductRoutes
{
    public static WebApplication AddProductRoutes(this WebApplication app)
    {
        app.MapGet("/v1/products", async ([FromServices] ProductDbContext context) =>
        {
            var products = await context.Products.ToListAsync();

            if (products is null)
                Results.NotFound();

            return Results.Ok(products);
        })
        .WithName("GetProductsAction")
        .RequireAuthorization();

        app.MapDelete("/v1/products/{id}", async ([FromServices] ProductDbContext context, int id) =>
        {
            var product = await context.Products.FindAsync(id);

            if (product is null)
                return Results.NotFound();

            context.Products.Remove(product);
            await context.SaveChangesAsync();

            return Results.NoContent();
        })
        .WithName("DeleteProductAction")
        .RequireAuthorization();

        app.MapPut("/v1/products/{id}", async ([FromServices] ProductDbContext context, [FromRoute] int id, [FromBody] ProductModel model) =>
        {
            var product = await context.Products.FindAsync(id);

            if (product is null)
                return Results.NotFound();

            product.Name = model.Name;
            product.Price = model.Price;

            await context.SaveChangesAsync();

            return Results.Ok(product);
        })
        .WithName("UpdateProductAction")
        .RequireAuthorization();

        app.MapPost("/v1/products", async ([FromServices] ProductDbContext context, [FromBody] ProductModel model) =>
        {
            if (model is not null)
            {
                var productEntity = new Product
                {
                    Name = model.Name,
                    Price = model.Price
                };

                Console.WriteLine(model.Name);

                await context.Products.AddAsync(productEntity);
                await context.SaveChangesAsync();

                return Results.Created($"/{productEntity.Id}", productEntity);
            }

            return Results.BadRequest("Invalid product data");
        })
        .WithName("CreateProductAction")
        .RequireAuthorization();

        return app;
    }
}
public static class Users
{
    public static Dictionary<string, string> UsersList()
    {
        return new Dictionary<string, string>
        {
            { "raphael", "123456" },
            { "batman", "123456" }
        };
    }
}