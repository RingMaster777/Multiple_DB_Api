using MyApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services
    .AddApplicationServices(builder.Configuration, builder.Environment)
    .AddControllers();

var app = builder.Build();

// Apply migrations and configure middleware
await app.ApplyMigrationsAsync();
app.UseApplicationPipeline()
   .MapHealthCheckEndpoint();

app.Run();
