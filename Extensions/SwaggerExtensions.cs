using Microsoft.OpenApi.Models;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        // Enables API endpoint exploration features which are crucial for the app to provide metadata about its HTTP endpoints to the Swagger UI.
        services.AddEndpointsApiExplorer();

        // Sets up the generation of Swagger documentation for the API.
        services.AddSwaggerGen(s => 
        {
            s.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "API",
                Version = "v1",
                Description = "A simple API"
            });
        });

        return services;
    }
}