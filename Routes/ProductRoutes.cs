using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public static class ProductRoutes
{
    private static string _filePath = "Documents";
    public static void AddProductRoutes(this WebApplication app)
    {
        app.MapGet("/", async ([FromServices] ProductDbContext context) =>
        {
            var products = await context.Products.ToListAsync();

            if (products is null)
                Results.NotFound();

            return Results.Ok(products);
        })
        .WithName("GetProductsAction")
        .RequireAuthorization();

        app.MapPost("/", async ([FromServices] ProductDbContext context, [FromBody] ProductModel model) =>
        {
            if (model is not null)
            {
                var productEntity = new Product
                {
                    Name = model.Name,
                    Price = model.Price
                };

                await context.Products.AddAsync(productEntity);
                await context.SaveChangesAsync();

                return Results.Created($"/{productEntity.Id}", productEntity);
            }

            return Results.BadRequest("Invalid product data");
        })
        .WithName("CreateProductAction")
        .RequireAuthorization();

        app.MapPost("/login", ([FromServices] IHttpContextAccessor accessor, [FromBody] UserModel model) =>
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

        app.MapPost("/files", ([FromForm] IFormFile file) => 
        {
            if(file.OpenReadStream().CanSeek)
            {
                file.OpenReadStream().Seek(0, SeekOrigin.Begin);
                file.OpenReadStream().CopyTo(File.Create($"{_filePath}/{file.FileName}.{GetFileExtension(file.FileName)}"));

                return Results.Created($"/files/{file.FileName}", file);
            }

            return Results.BadRequest("Invalid file data");
        })
        .WithName("UploadFileAction")
        .RequireAuthorization();

        app.MapGet("/files", async ([FromQuery] string fileName) =>
        {
            byte[] buffer = new byte[16 * 1024];
            int bytesRead;
            List<string> result = new List<string>();

            using (var stream = new FileStream($"{_filePath}/{fileName}.{GetFileExtension(fileName)}", FileMode.Open, FileAccess.Read))
            {
                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    result.Add(Encoding.UTF8.GetString(buffer, 0, bytesRead));
                }
            }

            // while ((bytesRead = file.OpenReadStream().ReadByte()) > 0)

            if(result is null || result.Count == 0)
                return Results.NotFound();

            var enumerator = result.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Console.WriteLine(enumerator.Current);
            }

            return Results.Ok(result);
        })
        .WithName("DownloadFileAction")
        .RequireAuthorization();
    }

    private static string GetFileExtension(string fileName)
    {
        return Path.GetExtension(fileName);
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