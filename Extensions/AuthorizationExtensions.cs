public static class AuthorizationExtensions
{
    public static IServiceCollection AddCustomAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options => {
            options.AddPolicy("Admin", policy => policy.RequireClaim("role", "admin"));
        });

        return services;
    }
}