using Pakturaly.Api;

var builder = WebApplication.CreateBuilder(args);

DependencyInjection.ConfigureConfiguration(builder.Configuration);
DependencyInjection.ConfigureHost(builder.Host, builder.Configuration);
DependencyInjection.ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();
DependencyInjection.ConfigureApplication(app, app.Environment);

app.MapGet("/test", (int? pageSize, int? page) => {
    return Results.Ok(new());
})
    .Produces(400)
    .WithName("GetProducts")
    .WithTags("Products")
    .WithSummary("Retrieve a list of products");

app.Run();
