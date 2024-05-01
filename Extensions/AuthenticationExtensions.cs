using Microsoft.AspNetCore.Authentication.Cookies;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddCustomAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(cookieOptions => {
                cookieOptions.Cookie.Name = "auth_cookie";
                cookieOptions.LoginPath = "/login";
                cookieOptions.LogoutPath = "/logout";
                cookieOptions.ExpireTimeSpan = TimeSpan.FromMinutes(30);
            });

        return services;
    }
}