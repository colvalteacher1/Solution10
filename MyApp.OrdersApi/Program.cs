using Microsoft.EntityFrameworkCore;
using MyApp.OrdersApi.Data;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("OrdersDatabase")
    ?? throw new InvalidOperationException("La chaîne de connexion OrdersDatabase est introuvable.");

var productsServiceUrl = builder.Configuration["ProductsService:BaseUrl"]
    ?? throw new InvalidOperationException("L’URL de ProductsService est introuvable.");

builder.Services.AddControllers();
builder.Services.AddDbContext<OrdersDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddHttpClient("ProductsService", client =>
{
    client.BaseAddress = new Uri(productsServiceUrl);
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
