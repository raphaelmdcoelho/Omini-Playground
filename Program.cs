var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAntiforgery();

builder.Services.AddSwagger().AddCustomAuthentication().AddCustomAuthorization().AddProductDatabase().AddOrderDatabase();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// UseSwagger() is about enabling your application to output the Swagger document in a machine-readable JSON format.
app.UseSwagger();
// is about providing a human-readable, interactive interface to explore and interact with the API using that JSON document.
app.UseSwaggerUI(options => { options.SwaggerEndpoint("/swagger/v1/swagger.json", "API"); });

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.AddAdminRoutes().AddProductRoutes().AddFileRoutes();

app.Run();