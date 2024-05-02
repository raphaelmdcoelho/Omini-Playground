using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

public static class AdminRoutes
{
    public static WebApplication AddAdminRoutes(this WebApplication app)
    {
        app.MapPost("/v1/login", ([FromServices] IHttpContextAccessor accessor, [FromBody] UserModel model) =>
        {
            var user = Users
                .UsersList()
                .Where(x => x.Key == model.Username && x.Value == model.Password)
                .Select(x => new UserModel(x.Key, x.Value))
                .SingleOrDefault();

            if (user is null)
            {
                return Results.BadRequest("Invalid username or password");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Username == "raphael" ? "admin" : "user")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            accessor.HttpContext!.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return Results.Ok("Logged in");
        })
        .WithName("LoginAction")
        .AllowAnonymous();

        app.MapPost("/v1/logout", ([FromServices] IHttpContextAccessor accessor) =>
        {
            accessor.HttpContext!.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Results.Ok("Logged out");
        })
        .WithName("LogoutAction")
        .RequireAuthorization();

        return app;
    }
}